// <copyright file="Automata.ExecutionPath.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly List<ExecutionPath> _executionPathRegistry = new();

		private void RegisterExecutionPath(ExecutionPath executionPath)
		{
			lock (_executionPathRegistry)
			{
				executionPath.Id = _executionPathRegistry.Count;

				_executionPathRegistry.Add(executionPath);

				if (ExecutionPathLookAheadLength < executionPath.LookAheadMatch.Length)
					ExecutionPathLookAheadLength = executionPath.LookAheadMatch.Length;
			}
		}

		private protected abstract class ExecutionPathBase
		{
		}

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private protected sealed class ExecutionPath : ExecutionPathBase
		{
			public static readonly ExecutionPath Invalid = new(null, Array.Empty<Node>(), -1);

			private readonly IPool<ExecutionPath> _forkExecutionPathPool;
			public readonly bool ContextEval;
			public readonly int[] EnterReturnSubGraphs;
			public readonly bool ForkPredicatePath;
			public readonly bool HasPrecedenceNodes;
			public readonly bool IsInvalid;
			public readonly MatchEntry[] LookAheadMatch;
			public readonly MatchEntry Match;
			public readonly Node[] Nodes;
			public readonly bool OutputEnd;
			public readonly bool OutputReturn;
			public readonly bool PassLazyNode;
			public readonly PrecedenceNode[] PrecedenceNodes;
			public readonly int StackDelta;
			public readonly int StackDepth;
			public readonly int Weight;
			public int Id;
			public ExecutionPath LookAheadPath;
			public Node Output;
			public Node PathSourceNode;
			public PredicateEntryBase Predicate;
			public ReferenceCounter ReferenceCount;
			public bool Safe;

			public ExecutionPath(Node pathSourceNode, Node[] route, int weight, params MatchEntry[] match)
			{
				PathSourceNode = pathSourceNode;
				Nodes = route;
				IsInvalid = Nodes == null || Nodes.Length == 0;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];
				OutputReturn = Output is ReturnRuleNode;
				OutputEnd = Output is EndRuleNode;

				if (match == null || match.Length == 0)
				{
					LookAheadMatch = Array.Empty<MatchEntry>();
				}
				else
				{
					Match = match[0];
					LookAheadMatch = match;
				}

				Weight = weight;

				CalcInfo(ref Weight, out PassLazyNode, out EnterReturnSubGraphs, out PrecedenceNodes, out HasPrecedenceNodes, out StackDepth, out StackDelta, out ContextEval);
			}

			public ExecutionPath(Node pathSourceNode, Node[] route, int weight, PredicateEntryBase predicate)
			{
				PathSourceNode = pathSourceNode;
				Nodes = route;
				IsInvalid = Nodes == null || Nodes.Length == 0;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];

				Debug.Assert(Output is PredicateNode);

				OutputReturn = false;
				Predicate = predicate;
				LookAheadMatch = Array.Empty<MatchEntry>();
				Weight = weight;

				CalcInfo(ref Weight, out PassLazyNode, out EnterReturnSubGraphs, out PrecedenceNodes, out HasPrecedenceNodes, out StackDepth, out StackDelta, out ContextEval);
			}

			public ExecutionPath(Automata<TInstruction, TOperand> automata, IPool<ExecutionPath> forkExecutionPathPool)
			{
				_forkExecutionPathPool = forkExecutionPathPool;

				Nodes = new Node[1];
				IsInvalid = false;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];
				OutputReturn = Output is ReturnRuleNode;
				OutputEnd = Output is EndRuleNode;
				EnterReturnSubGraphs = Array.Empty<int>();
				PrecedenceNodes = Array.Empty<PrecedenceNode>();
				ForkPredicatePath = true;
				ContextEval = false;
				LookAheadMatch = Array.Empty<MatchEntry>();
				Weight = 0;

				automata.RegisterExecutionPath(this);
			}

			private string DebuggerDisplay
			{
				get
				{
					if (IsInvalid)
						return "???";

					var operandStr = Match?.ToString() ?? "?";

					return SourceNode + " - {" + (OutputReturn ? "Ret" : operandStr) + "} - " + Output;
				}
			}

			private string DebugView
			{
				get { return string.Join("\n", Nodes.Select(n => n.ToString())); }
			}

			public bool IsForkExecutionPath => _forkExecutionPathPool != null;

			public bool IsPredicate => Predicate != null;

			public int PriorityIndex { get; set; }

			private Node SourceNode => IsInvalid ? null : Nodes[0];

			public ExecutionPath AddReference()
			{
				ReferenceCount.AddReference();

				return this;
			}

			private void CalcInfo(ref int weight, out bool passLazyNode, out int[] enterReturnSubGraphs, out PrecedenceNode[] precedenceNodes, out bool hasPrecedenceNodes, out int stackDepth, out int stackDelta, out bool contextEval)
			{
				contextEval = Predicate == null;
				passLazyNode = false;
				stackDepth = 0;
				stackDelta = 0;

				if (Nodes == null || Nodes.Length == 0)
				{
					if (weight == -1)
						weight = 0;

					enterReturnSubGraphs = Array.Empty<int>();
					precedenceNodes = Array.Empty<PrecedenceNode>();
					hasPrecedenceNodes = false;

					return;
				}

				var flags = 0;
				List<int> enterReturnSubGraphList = null;
				List<PrecedenceNode> precedenceNodeList = null;

				if (weight == -1)
				{
					weight = 0;
					var firstNode = Nodes[0];
					var current = firstNode;

					foreach (var node in Nodes)
					{
						flags |= node.Flags;

						if (node is EnterRuleNode enterRuleNode)
						{
							stackDepth++;
							stackDelta++;

							var rid = enterRuleNode.SubGraph.RId == -1 ? enterRuleNode.SubGraph.Id : enterRuleNode.SubGraph.RId;

							(enterReturnSubGraphList ??= new List<int>()).Add(rid);
						}
						else if (node is ReturnRuleNode)
						{
							stackDelta--;
							(enterReturnSubGraphList ??= new List<int>()).Add(-1);
						}
						else if (node is PrecedenceNode precedenceNode)
						{
							precedenceNodeList ??= new List<PrecedenceNode>();
							precedenceNodeList.Add(precedenceNode);
						}

						if (ReferenceEquals(node, firstNode))
							continue;

						var edge = current.GetTargetNodeEdge(node);

						if (edge != null)
							weight = Math.Max(weight, edge.Weight);

						current = node;
					}
				}
				else
				{
					// ReSharper disable once LoopCanBeConvertedToQuery
					foreach (var node in Nodes)
					{
						if (node is EnterRuleNode enterRuleNode)
						{
							stackDepth++;
							stackDelta++;

							var rid = enterRuleNode.SubGraph.RId == -1 ? enterRuleNode.SubGraph.Id : enterRuleNode.SubGraph.RId;

							(enterReturnSubGraphList ??= new List<int>()).Add(rid);
						}
						else if (node is ReturnRuleNode)
						{
							stackDelta--;
							(enterReturnSubGraphList ??= new List<int>()).Add(-1);
						}
						else if (node is PrecedenceNode precedenceNode)
						{
							precedenceNodeList ??= new List<PrecedenceNode>();
							precedenceNodeList.Add(precedenceNode);
						}

						flags |= node.Flags;
					}
				}

				precedenceNodes = precedenceNodeList != null ? precedenceNodeList.ToArray() : Array.Empty<PrecedenceNode>();
				enterReturnSubGraphs = enterReturnSubGraphList != null ? enterReturnSubGraphList.ToArray() : Array.Empty<int>();

				passLazyNode = (flags & Node.Lazy) != 0;
				hasPrecedenceNodes = precedenceNodes.Length > 0;
			}

			private bool Equals(ExecutionPath other)
			{
				return Equals(Nodes, other.Nodes) && EntryEqualityComparer.Instance.Equals(Match, other.Match);
			}

			public override bool Equals(object obj)
			{
				if (obj is null)
					return false;

				return obj is ExecutionPath path && Equals(path);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((Nodes != null ? Nodes.GetHashCode() : 0) * 397) ^ (Match != null ? EntryEqualityComparer.Instance.GetHashCode(Match) : 0);
				}
			}

			public static ExecutionPath JoinPaths(IReadOnlyList<ExecutionPath> executionPaths)
			{
				return new ExecutionPath(executionPaths[0].PathSourceNode, executionPaths.SelectMany(p => p.Nodes).ToArray(), executionPaths.Max(p => p.Weight), executionPaths.SelectMany(p => p.LookAheadMatch).Where(m => m != null).ToArray());
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void MakeSafe()
			{
				MakeSafeSync();
			}

			private void MakeSafeSync()
			{
				lock (this)
				{
					if (Safe)
						return;

					foreach (var node in Nodes)
					{
						if (node is EnterRuleNode enterStateNode)
						{
							var subGraph = enterStateNode.SubGraph;
							var leaveStateNode = subGraph.LeaveNode;

							if (leaveStateNode.Safe == false)
								leaveStateNode.MakeSafe();
						}
					}

					if (Output is LeaveRuleNode == false)
					{
						if (Output.Safe == false)
							Output.MakeSafe();
					}

					Safe = true;
				}
			}

			public ExecutionPath Mount(Node startNode, PredicateNode predicate)
			{
				PathSourceNode = startNode;
				Nodes[0] = predicate;
				Predicate = predicate.PredicateEntry;
				Output = predicate;

				ReferenceCount.AddReference();

				return this;
			}

			public void ReCalcRId(Automata<TInstruction, TOperand> automata)
			{
				for (var i = 0; i < EnterReturnSubGraphs.Length; i++)
				{
					var subGraphId = EnterReturnSubGraphs[i];

					if (subGraphId is < 0 or > SubGraphIdMask)
						continue;

					var subGraph = automata._subGraphRegistry[subGraphId];

					if (subGraph.RId == -1)
						throw new InvalidOperationException();

					EnterReturnSubGraphs[i] = subGraph.RId;
				}
			}

			public void Release()
			{
				if (ForkPredicatePath == false)
					return;

				if (ReferenceCount.ReleaseReference() > 0)
					return;

				PathSourceNode = null;

				var predicateNode = (PredicateNode)Nodes[0];

				(predicateNode.PredicateEntry as IDisposable)?.Dispose();
				predicateNode.Release();

				Nodes[0] = null;
				Predicate = null;

				_forkExecutionPathPool.Release(this);
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}
		}
	}
}