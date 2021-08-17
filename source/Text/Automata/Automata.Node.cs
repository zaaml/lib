// <copyright file="Automata.Node.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Extensions;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly List<Node> _nodeRegistry = new();

		[UsedImplicitly]
		private Node GetNode(int nodeId)
		{
			return _nodeRegistry[nodeId];
		}

		private int RegisterNode(Node node)
		{
			if (node is PredicateNode)
				HasPredicates = true;

			var id = _nodeRegistry.Count;

			_nodeRegistry.Add(node);
			
			return id;
		}

		[DebuggerDisplay("{" + nameof(Name) + "}")]
		private protected abstract class Node : IEquatable<Node>
		{
			public const int EnterReturn = 1;
			public const int Lazy = 2;

			public Automata<TInstruction, TOperand> Automata;
			public readonly int Flags;
			public readonly Graph Graph;
			public readonly int Id;
			public readonly ThreadStatusKind ThreadStatusKind;
			private readonly Dictionary<Process.AutomataStack, Process.Thread.Dfa> _dfaDictionary = new(Process.AutomataStack.EqualityComparer);
			private volatile bool _buildingReturnPath;
			private volatile ExecutionPathLookup _executionPathLookup;
			private volatile ExecutionPath[] _executionPaths;
			private volatile ExecutionPath[] _finalExecutionPaths;
			private volatile ExecutionPath[] _returnPaths;
			public ExecutionPathLookup ExecutionPathLookupSafe;
			public ExecutionPath[] ExecutionPathSafe;
			public bool HasReturnPathSafe;
			public ExecutionPath[] ReturnPathsSafe;
			public bool Safe;

			private protected Node(ThreadStatusKind threadStatusKind)
			{
				Debug.Assert(threadStatusKind == ThreadStatusKind.Unexpected);

				ThreadStatusKind = ThreadStatusKind.Unexpected;
			}

			private Node(Automata<TInstruction, TOperand> automata, ThreadStatusKind threadStatusKind = ThreadStatusKind.Run)
			{
				Automata = automata;

				Id = automata.RegisterNode(this);

				if (this is EnterRuleNode || this is LeaveRuleNode)
					Flags |= EnterReturn;

				if (this is LazyNode)
					Flags |= Lazy;

				ThreadStatusKind = threadStatusKind;
			}

			protected Node(Automata<TInstruction, TOperand> automata, Graph graph, ThreadStatusKind threadStatusKind = ThreadStatusKind.Run) : this(automata, threadStatusKind)
			{
				Graph = graph;
				Graph?.AddNode(this);
			}

			public Process.Thread.Dfa GetDfaThread(Process.AutomataStack stack)
			{
				lock (_dfaDictionary)
				{
					if (_dfaDictionary.TryGetValue(stack, out var dfa))
						return dfa;

					var dfaStack = stack.CreateDfaStack(Automata._dfaAllocator);

					dfa = new Process.Thread.Dfa(this, dfaStack);

					_dfaDictionary.Add(dfaStack, dfa);

					return dfa;
				}
			}

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

						_executionPathLookup = new ExecutionPathLookup(Automata, BuildExecutionGraph());
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

			public bool HasReturn => Safe ? HasReturnPathSafe : ReturnPaths.Length > 0;

			public List<Edge> InEdges { get; } = new();

			protected abstract string KindString { get; }

			private bool LookAheadEnabled => Automata.LookAheadEnabled;

			[UsedImplicitly]
			public string Name => Graph != null ? Graph.Rule.Name + KindString : KindString;

			public List<Edge> OutEdges { get; } = new();

			public ExecutionPath[] ReturnPaths
			{
				get
				{
					if (_returnPaths != null)
						return _returnPaths;

					lock (this)
					{
						if (_returnPaths != null)
							return _returnPaths;

						if (_buildingReturnPath)
							return Array.Empty<ExecutionPath>();

						_buildingReturnPath = true;

						_returnPaths = BuildReturnPath();

						_buildingReturnPath = false;
					}

					return _returnPaths;
				}
			}

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

					if (executionPath.IsPredicate || executionPath.OutputReturn)
						continue;

					if (LookAheadEnabled)
					{
						var nodeSet = new HashSet<Node>(executionPath.Nodes) { this };

						while (output.ExecutionPaths.Length == 1 && output.ReturnPaths.Length == 0)
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

						executionPath.LookAheadPath = ExecutionPath.JoinPaths(jointPaths);
					}
					else if (output.ExecutionPaths.Length == 0 && output.ReturnPaths.Length == 1)
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
					Automata.RegisterExecutionPath(executionPath);

					if (executionPath.LookAheadPath != null)
						Automata.RegisterExecutionPath(executionPath.LookAheadPath);
				}

				_finalExecutionPaths = executionPaths;

				return _finalExecutionPaths;
			}

			public void ReCalcRId(Automata<TInstruction, TOperand> automata)
			{
				if (_executionPaths != null)
				{
					foreach (var executionPath in _executionPaths) 
						executionPath.ReCalcRId(automata);
				}

				if (_returnPaths != null)
				{
					foreach (var executionPath in _returnPaths) 
						executionPath.ReCalcRId(automata);
				}
			}

			private ExecutionPath[] BuildReturnPath()
			{
				var returnPaths = EnumerateReturnPaths().ToArray();

				foreach (var returnPath in returnPaths)
					Automata.RegisterExecutionPath(returnPath);

				return returnPaths;
			}

			private static void BuildRoute(EdgeDfsStack stack, Node outputNode, IReadOnlyList<int> returnPathCombination, List<NodeRoute> routes)
			{
				var weight = 0;
				var routeList = new List<Node>();

				var retPos = 0;

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
						routeList.AddRange(subGraph.Graph.BeginNode.ReturnPaths[returnPathCombination[retPos++]].Nodes);
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
						routeList.AddRange(subGraph.Graph.BeginNode.ReturnPaths[returnPathCombination[retPos]].Nodes);
					}

					routeList.Add(outputNode);
				}

				routes.Add(new NodeRoute(routeList.ToArray(), weight));
			}

			private static NodeRoute[] BuildRoute(EdgeDfsStack stack, Node outputNode)
			{
				List<NodeRoute> routes = new();
				List<int> returnPathCombinations = null;

				for (var index = 1; index < stack.Count; index++)
				{
					ref var edgeDfs = ref stack.Array[index];
					var node = edgeDfs.Node;

					if (node is not LeaveRuleNode leaveStateNode)
						continue;

					returnPathCombinations ??= new List<int>();

					var subGraph = leaveStateNode.SubGraph;
					var returnPathsLength = subGraph.Graph.BeginNode.ReturnPaths.Length;

					returnPathCombinations.Add(returnPathsLength);
				}

				if (returnPathCombinations == null)
					BuildRoute(stack, outputNode, null, routes);
				else
					returnPathCombinations.VisitCartesianCoordinates(rc => BuildRoute(stack, outputNode, rc, routes));

				return routes.ToArray();
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
					_returnPaths = source.ReturnPaths;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node EnsureSafe()
			{
				if (Safe)
					return this;

				MakeSafeSynced();

				return this;
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

								if (beginNode.HasReturn)
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
						if (nextEdge.Terminal)
						{
							foreach (var route in BuildRoute(stack, nextNode))
							{
								if (nextEdge.OperandMatch != null)
									result.Add(new ExecutionPath(this, route.Nodes, route.Weight, nextEdge.OperandMatch));
								else if (nextEdge.PredicateMatch != null)
									result.Add(new ExecutionPath(this, route.Nodes, route.Weight, nextEdge.PredicateMatch));
								else if (nextEdge.TargetNode is EndRuleNode)
									result.Add(new ExecutionPath(this, route.Nodes, route.Weight));
								else
									throw new InvalidOperationException();
							}

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

				return result.Concat(ReturnPaths.Reverse()).ToArray();
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

							if (nodeVisitor.Add(enterStateNode.SubGraph.LeaveNode) && beginNode.HasReturn)
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
							foreach (var route in BuildRoute(stack, nextNode))
							{
								if (route.Nodes.Length > 0)
									result.Add(new ExecutionPath(this, route.Nodes, route.Weight));
							}

							RemoveVisitedNode(nodeVisitor, nextNode);
						}
						else if (nextEdge.Terminal == false)
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
			public ExecutionPath[] GetExecutionPathsFastSafe(int intOperand)
			{
				return ExecutionPathLookupSafe.GetExecutionPathGroupFast(intOperand).ExecutionPaths;
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

					ReturnPathsSafe = ReturnPaths;
					HasReturnPathSafe = ReturnPaths.Length > 0;
					ExecutionPathSafe = ExecutionPaths;
					ExecutionPathLookupSafe = ExecutionPathLookup;

					Safe = true;
				}
			}

			private static void RemoveVisitedNode(HashSet<Node> nodeVisitor, Node node)
			{
				nodeVisitor.Remove(node);

				if (node is EnterRuleNode enterStateNode)
					nodeVisitor.Remove(enterStateNode.SubGraph.LeaveNode);
			}

			public override string ToString()
			{
				return Name;
			}

			public bool Equals(Node other)
			{
				return ReferenceEquals(this, other);
			}

			private readonly struct NodeRoute
			{
				public NodeRoute(Node[] nodes, int weight)
				{
					Nodes = nodes;
					Weight = weight;
				}

				public readonly Node[] Nodes;

				public readonly int Weight;
			}

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
				private const int DefaultCapacity = 8;
				private static readonly EdgeDfs[] EmptyArray = System.Array.Empty<EdgeDfs>();

				public EdgeDfs[] Array;
				public int Count;

				public EdgeDfsStack()
				{
					Array = EmptyArray;
					Count = 0;
				}

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
			}
		}

		private sealed class NodesEqualityComparer : IEqualityComparer<ExecutionPath>
		{
			public static readonly NodesEqualityComparer Instance = new();

			private NodesEqualityComparer()
			{
			}

			public bool Equals(ExecutionPath x, ExecutionPath y)
			{
				// ReSharper disable once PossibleNullReferenceException
				return x.Nodes.SequenceEqual(y.Nodes);
			}

			public int GetHashCode(ExecutionPath obj)
			{
				return obj.Nodes.GetHashCode();
			}
		}
	}
}