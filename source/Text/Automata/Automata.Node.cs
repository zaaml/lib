// <copyright file="Automata.Node.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Fields

		private readonly List<Node> _nodeRegistry = new List<Node>();

		#endregion

		#region Methods

		[UsedImplicitly]
		private Node GetNode(int nodeId)
		{
			return _nodeRegistry[nodeId];
		}

		private void RegisterNode(Node node)
		{
			node.Id = _nodeRegistry.Count;

			_nodeRegistry.Add(node);
		}

		#endregion

		#region Nested Types

		private protected class NodeBase
		{
		}

		[DebuggerDisplay("{" + nameof(Name) + "}")]
		private abstract class Node : NodeBase, IEquatable<Node>
		{
			#region Static Fields and Constants

			public const int EnterReturn = 1;
			public const int Lazy = 2;

			#endregion

			#region Fields

			private readonly Automata<TInstruction, TOperand> _automata;
			public readonly int Flags;
			public readonly Graph Graph;
			private volatile ExecutionPathLookup _executionPathLookup;
			private volatile ExecutionPath[] _executionPaths;
			private volatile ExecutionPath[] _finalExecutionPaths;
			private volatile ExecutionPath _returnPath;
			private volatile bool _buildingReturnPath;

			#endregion

			public bool Safe;
			public ExecutionPath ReturnPathSafe;
			public bool HasReturnPathSafe;
			public ExecutionPathLookup ExecutionPathLookupSafe;
			public ExecutionPath[] ExecutionPathSafe;

			#region Ctors

			private Node(Automata<TInstruction, TOperand> automata)
			{
				_automata = automata;

				automata.RegisterNode(this);

				if (this is EnterRuleNode || this is LeaveRuleNode)
					Flags |= EnterReturn;

				if (this is LazyNode)
					Flags |= Lazy;
			}

			protected Node(Automata<TInstruction, TOperand> automata, Graph graph) : this(automata)
			{
				Graph = graph;
				Graph?.AddNode(this);
			}

			#endregion

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void MakeSafe()
			{
				MakeSafeSynced();
			}

			private void MakeSafeSynced()
			{
				lock (this)
				{
					if (Safe)
						return;

					ReturnPathSafe = ReturnPath;
					HasReturnPathSafe = HasReturn;
					ExecutionPathSafe = ExecutionPaths;
					ExecutionPathLookupSafe = ExecutionPathLookup;
					
					Safe = true;
				}
			}

			#region Properties

			public List<Edge> OutEdges { get; } = new List<Edge>();

			public List<Edge> InEdges { get; } = new List<Edge>();

			private ExecutionPathLookup ExecutionPathLookup
			{
				get
				{
					if (_executionPathLookup != null)
						return _executionPathLookup;

					lock (this)
					{
						if (_executionPathLookup != null)
							return _executionPathLookup;

						_executionPathLookup = new ExecutionPathLookup(BuildExecutionGraph());
					}

					return _executionPathLookup;
				}
			}

			public ExecutionPath[] ExecutionPaths
			{
				get
				{
					if (_executionPaths != null)
						return _executionPaths;

					lock (this)
					{
						if (_executionPaths != null)
							return _executionPaths;

						_executionPaths = EnumerateExecutionPaths();
					}

					return _executionPaths;
				}
			}

			public bool HasReturn => ReturnPath.IsInvalid == false;

			public int Id;

			public int Index { get; set; }

			protected abstract string KindString { get; }

			private bool LookAheadEnabled => _automata.LookAheadEnabled;

			[UsedImplicitly] public string Name => Graph != null ? Graph.State.Name + KindString : KindString;

			public ExecutionPath ReturnPath
			{
				get
				{
					if (_returnPath != null)
						return _returnPath;

					lock (this)
					{
						if (_returnPath != null)
							return _returnPath;

						if (_buildingReturnPath)
							return ExecutionPath.Invalid;

						_buildingReturnPath = true;
						_returnPath = BuildReturnPath();
						_buildingReturnPath = false;
					}

					return _returnPath;
				}
			}

			#endregion

			#region Methods

			private void AddEdge(Edge edge)
			{
				OutEdges.Add(edge);
				edge.TargetNode.InEdges.Add(edge);
			}

			private ExecutionPath[] BuildExecutionGraph()
			{
				if (_finalExecutionPaths != null)
					return _finalExecutionPaths;

				_finalExecutionPaths = Array.Empty<ExecutionPath>();

				var executionPaths = ExecutionPaths;
				var jointPaths = new List<ExecutionPath>();

				for (var index = 0; index < executionPaths.Length; index++)
				{
					jointPaths.Clear();

					var executionPath = executionPaths[index];
					var output = executionPath.Output;

					executionPath.PriorityIndex = index;

					if (executionPath.IsPredicate)
						continue;

					if (LookAheadEnabled)
					{
						var nodeSet = new HashSet<Node>(executionPath.Nodes) { this };

						while (output.ExecutionPaths.Length == 1 && output.ReturnPath.IsInvalid)
						{
							var path = output.ExecutionPaths[0];

							if (path.IsPredicate)
								break;

							if (path.Nodes.All(nodeSet.Add) == false)
								break;

							if (jointPaths.Count == 0)
								jointPaths.Add(executionPath);

							jointPaths.Add(path);

							output = path.Output;
						}

						if (jointPaths.Count == 0)
							continue;

						//if (output.ExecutionPaths.Length == 0 && output.ReturnPath.IsInvalid == false) 
						//	jointPaths.Add(output.ReturnPath);

						executionPath.LookAhead = ExecutionPath.JoinPaths(jointPaths);
					}
					else if (output.ExecutionPaths.Length == 0 && output.ReturnPath.IsInvalid == false)
					{
						//jointPaths.Add(executionPath);
						//jointPaths.Add(output.ReturnPath);

						//var joinPaths = ExecutionPath.JoinPaths(jointPaths);

						//joinPaths.PriorityIndex = index;

						//executionPaths[index] = joinPaths;
					}
				}

				foreach (var executionPath in executionPaths)
				{
					_automata.RegisterExecutionPath(executionPath);

					if (executionPath.LookAhead != null)
						_automata.RegisterExecutionPath(executionPath.LookAhead);
				}

				_finalExecutionPaths = executionPaths;

				return _finalExecutionPaths;
			}

			private ExecutionPath BuildReturnPath()
			{
				var returnPaths = EnumerateReturnPaths().ToList();
				var returnPath = ExecutionPath.Invalid;

				if (returnPaths.Count != 1)
					return returnPath;

				returnPath = returnPaths[0];

				_automata.RegisterExecutionPath(returnPath);

				return returnPath;
			}

			private static Node[] BuildRoute(EdgeDfsStack stack, Node outputNode, out int weight)
			{
				weight = 0;
				var routeList = new List<Node>();

				for (var index = 1; index < stack.Count; index++)
				{
					ref var edgeDfs = ref stack.Array[index];
					var node = edgeDfs.Node;

					if (node.OutEdges.Count > 0)
						weight = Math.Max(weight, node.OutEdges[edgeDfs.Index].Weight);

					if (node is LeaveRuleNode leaveStateNode)
					{
						var subGraph = leaveStateNode.SubGraph;

						routeList.Add(subGraph.EnterNode);
						routeList.Add(subGraph.Graph.BeginNode);
						routeList.AddRange(subGraph.Graph.BeginNode.ReturnPath.Nodes);
					}

					routeList.Add(node);
				}

				{
					var lastNode = routeList.Count > 0 ? routeList[routeList.Count - 1] : null;

					if (lastNode?.OutEdges.Count > 0)
						weight = Math.Max(weight, lastNode.GetTargetNodeEdge(outputNode).Weight);

					if (outputNode is LeaveRuleNode leaveStateNode)
					{
						var subGraph = leaveStateNode.SubGraph;

						routeList.Add(subGraph.EnterNode);
						routeList.Add(subGraph.Graph.BeginNode);
						routeList.AddRange(subGraph.Graph.BeginNode.ReturnPath.Nodes);
					}

					routeList.Add(outputNode);
				}

				return routeList.ToArray();
			}

			public Node Connect(Node node, int weight = 0)
			{
				AddEdge(new Edge(this, node, weight));

				return node;
			}

			public void Connect(Node node, MatchEntry operand)
			{
				AddEdge(new Edge(this, node, operand));
			}

			public void Connect(Node node, PredicateEntryBase predicate)
			{
				AddEdge(new Edge(this, node, predicate));
			}

			public void CopyLookup(Node source)
			{
				lock (this)
				{
					_executionPathLookup = source.ExecutionPathLookup;
					_executionPaths = source.ExecutionPaths;
					_returnPath = source.ReturnPath;
				}
			}

			private ExecutionPath[] EnumerateExecutionPaths()
			{
				var result = new List<ExecutionPath>();
				var startNode = this;
				var nodeVisitor = new HashSet<Node>();
				var stack = new EdgeDfsStack();

				stack.Push(new EdgeDfs(startNode, -1));

				while (stack.Count > 0)
				{
					ref var peek = ref stack.Peek();
					var peekNode = peek.Node;

					switch (peekNode)
					{
						case EnterRuleNode enterStateNode:
						{
							var subGraph = enterStateNode.SubGraph;
							var beginNode = subGraph.Graph.BeginNode;

							if (peek.Index == -1)
							{
								peek.Index = 0;

								if (nodeVisitor.Add(beginNode))
									stack.Push(new EdgeDfs(beginNode, -1));
							}
							else
							{
								nodeVisitor.Remove(peekNode);

								stack.Pop();

								if (beginNode.ReturnPath.IsInvalid == false)
									stack.Push(new EdgeDfs(enterStateNode.SubGraph.LeaveNode, -1));
							}

							continue;
						}
					}

					var nextEdgeIndex = peek.Index + 1;
					var nextEdge = nextEdgeIndex < peekNode.OutEdges.Count ? peekNode.OutEdges[nextEdgeIndex] : null;
					var nextNode = nextEdge?.TargetNode;

					stack.Peek().Index = nextEdgeIndex;

					if (nextNode != null && nodeVisitor.Add(nextNode))
					{
						if (nextEdge.IsMatch)
						{
							var route = BuildRoute(stack, nextNode, out var weight);

							if (route.Length > 0)
							{
								if (nextEdge.OperandMatch != null)
									result.Add(new ExecutionPath(this, route, weight, nextEdge.OperandMatch));
								else if (nextEdge.PredicateMatch != null)
									result.Add(new ExecutionPath(this, route, weight, nextEdge.PredicateMatch));
								else
									throw new InvalidOperationException();
							}
							else
								throw new InvalidOperationException();

							RemoveVisitedNode(nodeVisitor, nextNode);
						}
						else
							stack.Push(new EdgeDfs(nextNode, -1));
					}
					else if (nextEdge == null)
					{
						var node = stack.Pop().Node;

						RemoveVisitedNode(nodeVisitor, node);
					}
				}

				return result.ToArray();
			}

			private List<ExecutionPath> EnumerateReturnPaths()
			{
				var result = new List<ExecutionPath>();
				var nodeVisitor = new HashSet<Node>();
				var stack = new EdgeDfsStack();

				nodeVisitor.Add(this);

				stack.Push(new EdgeDfs(this, -1));

				while (stack.Count > 0)
				{
					ref var peek = ref stack.Peek();
					var peekNode = peek.Node;

					switch (peekNode)
					{
						case EnterRuleNode enterStateNode:
						{
							var subGraph = enterStateNode.SubGraph;
							var beginNode = subGraph.Graph.BeginNode;

							if (nodeVisitor.Add(enterStateNode.SubGraph.LeaveNode) && beginNode.ReturnPath.IsInvalid == false)
							{
								stack.Pop();
								stack.Push(new EdgeDfs(enterStateNode.SubGraph.LeaveNode, -1));

								continue;
							}

							break;
						}
					}

					var nextEdgeIndex = peek.Index + 1;
					var nextEdge = nextEdgeIndex < peekNode.OutEdges.Count ? peekNode.OutEdges[nextEdgeIndex] : null;
					var nextNode = nextEdge?.TargetNode;

					stack.Peek().Index = nextEdgeIndex;

					if (nextNode != null && nodeVisitor.Add(nextNode))
					{
						if (nextNode is ReturnRuleNode)
						{
							var route = BuildRoute(stack, nextNode, out var weight);

							if (route.Length > 0)
								result.Add(new ExecutionPath(this, route, weight));

							RemoveVisitedNode(nodeVisitor, nextNode);
						}
						else if (nextEdge.IsMatch == false)
							stack.Push(new EdgeDfs(nextNode, -1));
					}
					else if (nextEdge == null)
					{
						var node = stack.Pop().Node;

						RemoveVisitedNode(nodeVisitor, node);
					}
				}

				return result;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPath[] GetExecutionPaths(int intOperand)
			{
				return ExecutionPathLookup.GetExecutionPathGroup(intOperand).ExecutionPaths;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPath[] GetExecutionPaths()
			{
				return ExecutionPathLookup.GetExecutionPathGroup().ExecutionPaths;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPath[] GetExecutionPathsFastSafe(int intOperand)
			{
				return ExecutionPathLookupSafe.GetExecutionPathGroupFast(intOperand).ExecutionPaths;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPath[] GetExecutionPathsFast()
			{
				return ExecutionPathLookup.GetExecutionPathGroupFast().ExecutionPaths;
			}

			public Edge GetTargetNodeEdge(Node targetNode)
			{
				// ReSharper disable once ForCanBeConvertedToForeach
				// ReSharper disable once LoopCanBeConvertedToQuery
				for (var index = 0; index < OutEdges.Count; index++)
				{
					var edge = OutEdges[index];

					if (ReferenceEquals(edge.TargetNode, targetNode))
						return edge;
				}

				return null;
			}

			private static void ProcessPath(ExecutionPath executionPath)
			{
				//foreach (var pathNode in executionPath.Nodes.OfType<EnterStateNode>())
				//	pathNode.SubGraph.LeaveNode.BuildExecutionGraph();

				//executionPath.OriginalPath?.Output.BuildExecutionGraph();
				//executionPath.Output.BuildExecutionGraph();
			}

			private static void RemoveVisitedNode(HashSet<Node> nodeVisitor, Node node)
			{
				nodeVisitor.Remove(node);

				if (node is EnterRuleNode enterStateNode)
					nodeVisitor.Remove(enterStateNode.SubGraph.LeaveNode);
			}

			public bool Equals(Node other)
			{
				return ReferenceEquals(this, other);
			}

			public override string ToString()
			{
				return Name;
			}

			#endregion

			#region Nested Types

			private struct EdgeDfs
			{
				#region Ctors

				public EdgeDfs(Node node, int index)
				{
					Node = node;
					Index = index;
				}

				#endregion

				#region Properties

				public int Index;
				public Node Node;

				#endregion

				#region Methods

				public override string ToString()
				{
					return $"{Node}, {Index}";
				}

				#endregion
			}

			private class EdgeDfsStack
			{
				#region Static Fields and Constants

				private const int DefaultCapacity = 8;
				private static readonly EdgeDfs[] EmptyArray = new EdgeDfs[0];

				#endregion

				#region Fields

				public EdgeDfs[] Array;
				public int Count;

				#endregion

				#region Ctors

				public EdgeDfsStack()
				{
					Array = EmptyArray;
					Count = 0;
				}

				#endregion

				#region Methods

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Clear()
				{
					System.Array.Clear(Array, 0, Count);

					Count = 0;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public EdgeDfs Get(int index) => Array[index];

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ref EdgeDfs Peek()
				{
#if DEBUG
					if (Count == 0)
						Error.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
#endif
					return ref Array[Count - 1];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public EdgeDfs Pop()
				{
#if DEBUG
					if (Count == 0)
						Error.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
#endif
					return Array[--Count];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Push(EdgeDfs item)
				{
					if (Count == Array.Length)
					{
						var newArray = new EdgeDfs[Array.Length == 0 ? DefaultCapacity : 2 * Array.Length];

						System.Array.Copy(Array, 0, newArray, 0, Count);

						Array = newArray;
					}

					Array[Count++] = item;
				}

				#endregion
			}

			#endregion
		}

		private sealed class NodesEqualityComparer : IEqualityComparer<ExecutionPath>
		{
			#region Static Fields and Constants

			public static readonly NodesEqualityComparer Instance = new NodesEqualityComparer();

			#endregion

			#region Ctors

			private NodesEqualityComparer()
			{
			}

			#endregion

			#region Interface Implementations

			#region IEqualityComparer<Automata<TInstruction,TOperand>.ExecutionPath>

			public bool Equals(ExecutionPath x, ExecutionPath y)
			{
				// ReSharper disable once PossibleNullReferenceException
				return x.Nodes.SequenceEqual(y.Nodes);
			}

			public int GetHashCode(ExecutionPath obj)
			{
				return obj.Nodes.GetHashCode();
			}

			#endregion

			#endregion
		}

		#endregion
	}
}