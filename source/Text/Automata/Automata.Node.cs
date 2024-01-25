// <copyright file="Automata.Node.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Collections;

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

		private protected abstract class Node : IEquatable<Node>
		{
			public const byte EnterReturn = 1;
			public const byte Lazy = 2;
			private const byte ReturnPathBuilding = 4;
			private const byte SafeFlag = 8;
			private const byte HasReturnPathSafeFlag = 16;
			public readonly int Id;
			public readonly SyntaxGraph SyntaxGraph;
			public readonly ThreadStatusKind ThreadStatusKind;

			private IntTrie<DfaState> _dfaTrie;

			private volatile ExecutionPathLookup _executionPathLookup;
			private volatile ExecutionPath[] _executionPaths;
			private volatile ExecutionPath[] _finalExecutionPaths;
			private volatile ExecutionPath[] _returnPaths;

			public Automata<TInstruction, TOperand> Automata;
			public DfaState Dfa;
			public int DfaDepth = DfaBuilder.DefaultNextDepth;
			public byte Flags;

			private protected Node(ThreadStatusKind threadStatusKind)
			{
				Debug.Assert(threadStatusKind == ThreadStatusKind.Unexpected);

				ThreadStatusKind = ThreadStatusKind.Unexpected;
			}

			private Node(Automata<TInstruction, TOperand> automata, ThreadStatusKind threadStatusKind = ThreadStatusKind.Run)
			{
				Automata = automata;

				Id = automata.RegisterNode(this);

				if (this is EnterSyntaxNode || this is LeaveSyntaxNode)
					Flags |= EnterReturn;

				if (this is LazyNode)
					Flags |= Lazy;

				ThreadStatusKind = threadStatusKind;
			}

			protected Node(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, ThreadStatusKind threadStatusKind = ThreadStatusKind.Run) : this(automata, threadStatusKind)
			{
				SyntaxGraph = syntaxGraph;
				SyntaxGraph?.AddNode(this);
			}

			public IntTrie<DfaState> DfaTrie => _dfaTrie ??= new IntTrie<DfaState>();

			private bool ReturnPathsBuilding
			{
				get { return (Flags & ReturnPathBuilding) != 0; }
				set
				{
					if (value)
						Flags |= ReturnPathBuilding;
					else
						Flags &= unchecked((byte)~ReturnPathBuilding);
				}
			}

			public bool HasReturnPathSafe
			{
				get { return (Flags & HasReturnPathSafeFlag) != 0; }
				set
				{
					if (value)
						Flags |= HasReturnPathSafeFlag;
					else
						Flags &= unchecked((byte)~HasReturnPathSafeFlag);
				}
			}

			public bool Safe
			{
				get { return (Flags & SafeFlag) != 0; }
				set
				{
					if (value)
						Flags |= SafeFlag;
					else
						Flags &= unchecked((byte)~SafeFlag);
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

			//public List<Edge> InEdges { get; } = new();

			protected abstract string KindString { get; }

			private bool LookAheadEnabled => Automata.LookAheadEnabled;

			[UsedImplicitly]
			public string Name => SyntaxGraph != null ? SyntaxGraph.Syntax.Name + KindString : KindString;

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

						if (ReturnPathsBuilding)
							return Array.Empty<ExecutionPath>();

						ReturnPathsBuilding = true;

						_returnPaths = ExecutionPaths.Where(e => e.OutputReturn).ToArray();

						ReturnPathsBuilding = false;
					}

					return _returnPaths;
				}
			}

			public bool Equals(Node other)
			{
				return ReferenceEquals(this, other);
			}

			private void AddEdge(Edge edge)
			{
				OutEdges.Add(edge);
				//edge.TargetNode.InEdges.Add(edge);
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

						executionPath.LookAheadPath = JoinPaths(jointPaths);
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

				_finalExecutionPaths = executionPaths;

				return _finalExecutionPaths;
			}

			private ExecutionPath JoinPaths(IReadOnlyList<ExecutionPath> executionPaths)
			{
				return Automata.CreateExecutionPath(executionPaths[0].PathSourceNode, executionPaths.SelectMany(p => p.Nodes).ToArray(), executionPaths.SelectMany(p => p.LookAheadMatch).Where(m => m != null).ToArray());
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

			private static NodeRoute BuildRoute(EdgeDfsStack stack, Node outputNode)
			{
				var nodes = new List<Node>();

				for (var index = 1; index < stack.Count; index++)
				{
					ref var edgeDfs = ref stack.Array[index];
					var node = edgeDfs.Node;

					nodes.Add(node);
				}

				nodes.Add(outputNode);

				return new NodeRoute(nodes.ToArray());
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
			public void EnsureSafe()
			{
				if (Safe)
					return;

				MakeSafeSynced();
			}

			private ExecutionPath[] EnumerateExecutionPaths()
			{
				var result = new List<ExecutionPath>();
				var startNode = this;
				var nodeVisitor = new HashSet<Node>();
				var stack = new EdgeDfsStack();
				var subGraphStack = new Stack<SubGraph>();

				stack.Push(new EdgeDfs(startNode, -1));

				while (stack.Count > 0)
				{
					ref var peek = ref stack.Peek();
					var peekNode = peek.Node;

					switch (peekNode)
					{
						case EnterSyntaxNode enterRuleNode:
						{
							var subGraph = enterRuleNode.SubGraph;
							var beginNode = subGraph.SyntaxGraph.BeginNode;

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
							}

							continue;
						}
					}

					var nextEdgeIndex = peek.Index + 1;
					Edge? nextEdgeNull = nextEdgeIndex < peekNode.OutEdges.Count ? peekNode.OutEdges[nextEdgeIndex] : null;
					var nextNode = nextEdgeNull?.TargetNode;

					stack.Peek().Index = nextEdgeIndex;

					if (nextNode != null && nodeVisitor.Add(nextNode))
					{
						var nextEdge = nextEdgeNull.Value;
						if (nextEdge.Terminal)
						{
							var route = BuildRoute(stack, nextNode);

							if (nextEdge.OperandMatch != null)
								result.Add(Automata.CreateExecutionPath(this, route.Nodes, nextEdge.OperandMatch));
							else if (nextEdge.PredicateMatch != null)
								result.Add(Automata.CreateExecutionPath(this, route.Nodes, nextEdge.PredicateMatch));
							else if (nextEdge.TargetNode is ExitSyntaxNode)
								result.Add(Automata.CreateExecutionPath(this, route.Nodes));
							else
								throw new InvalidOperationException();

							RemoveVisitedNode(nodeVisitor, nextNode);
						}
						else if (nextNode is ReturnSyntaxNode)
						{
							subGraphStack.Clear();

							for (var index = 0; index < stack.Count - 1; index++)
							{
								ref var edgeDfs = ref stack.Array[index];
								var node = edgeDfs.Node;

								switch (node)
								{
									case EnterSyntaxNode enterRuleNode:
										subGraphStack.Push(enterRuleNode.SubGraph);

										break;
									case ReturnSyntaxNode:
										subGraphStack.Pop();

										break;
								}
							}

							if (subGraphStack.Count > 0)
							{
								var subGraph = subGraphStack.Pop();
								var leaveNode = subGraph.LeaveNode;

								if (nodeVisitor.Add(leaveNode) && ReferenceEquals(leaveNode, this) == false)
								{
									stack.Push(new EdgeDfs(nextNode, 0));
									stack.Push(new EdgeDfs(leaveNode, -1));
								}

								continue;
							}

							var route = BuildRoute(stack, nextNode);

							result.Add(Automata.CreateExecutionPath(this, route.Nodes));

							RemoveVisitedNode(nodeVisitor, nextNode);
						}
						else
							stack.Push(new EdgeDfs(nextNode, -1));
					}
					else if (nextEdgeNull == null)
					{
						var node = stack.Pop().Node;

						RemoveVisitedNode(nodeVisitor, node);
					}
				}

				return result.ToArray();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPath[] GetExecutionPaths(int intOperand)
			{
				return ExecutionPathLookup.GetExecutionPathGroup(intOperand).ExecutionPaths;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPath[] GetExecutionPathsFastSafe(int intOperand)
			{
				return _executionPathLookup.GetExecutionPathGroupFast(intOperand).ExecutionPaths;
			}

			public Edge? GetTargetNodeEdge(Node targetNode)
			{
				foreach (var edge in OutEdges)
				{
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

					HasReturnPathSafe = ReturnPaths.Length > 0;
					_ = ExecutionPaths;
					_ = ExecutionPathLookup;
					Dfa = Automata._dfaBuilderInstance.CreateNodeDfa(this);
					Safe = true;
				}
			}

			private static void RemoveVisitedNode(HashSet<Node> nodeVisitor, Node node)
			{
				nodeVisitor.Remove(node);

				if (node is EnterSyntaxNode enterStateNode)
					nodeVisitor.Remove(enterStateNode.SubGraph.LeaveNode);
			}

			public override string ToString()
			{
				return $"[{Id}] {Name}";
			}

			public void SortEdges()
			{
				var sortedEdges = OutEdges.OrderByDescending(e => e.Weight).ToList();

				OutEdges.Clear();
				OutEdges.AddRange(sortedEdges);
				OutEdges.TrimExcess();
			}

			private readonly struct NodeRoute
			{
				public NodeRoute(Node[] nodes)
				{
					Nodes = nodes;
				}

				public readonly Node[] Nodes;
			}

			private struct EdgeDfs
			{
				public EdgeDfs(Node node, int index)
				{
					Node = node;
					Index = index;
				}

				public int Index;
				public Node Node;

				public override string ToString()
				{
					return $"{Node}, {Index}";
				}
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
					Count--;

					var node = Array[Count];

					Array[Count] = default;

					return node;
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