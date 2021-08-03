// <copyright file="Automata.ExecutionPath.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly List<ExecutionPath> _executionPathRegistry = new List<ExecutionPath>();

		private void RegisterExecutionPath(ExecutionPath executionPath)
		{
			lock (_executionPathRegistry)
			{
				executionPath.Id = _executionPathRegistry.Count;

				_executionPathRegistry.Add(executionPath);

				lock (_executionPathMethodsDictionary)
					foreach (var kv in _executionPathMethodsDictionary)
						kv.Value.ResizeExecutionPaths(executionPath);
			}
		}

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private sealed class ExecutionPath
		{
			public static readonly ExecutionPath Invalid = new ExecutionPath(null, new Node[0], -1);

			private readonly IPool<ExecutionPath> _forkExecutionPathPool;
			public readonly Node[] EnterReturnNodes;
			public readonly bool ForkPredicatePath;
			public readonly bool IsInvalid;
			public readonly bool IsReturnPath;
			public ExecutionPath LookAhead;
			public readonly MatchEntry[] LookAheadMatch;
			public readonly MatchEntry Match;
			public readonly Node[] Nodes;
			public readonly Node Output;
			public readonly bool PassLazyNode;
			public readonly int StackEvalDelta;
			public readonly int Weight;
			public int Id;
			public Node PathSourceNode;
			public PredicateEntryBase Predicate;
			public ReferenceCounter ReferenceCount = new ReferenceCounter();
			public bool Safe;

			public ExecutionPath(Node pathSourceNode, Node[] route, int weight, params MatchEntry[] match)
			{
				PathSourceNode = pathSourceNode;
				Nodes = route;
				IsInvalid = Nodes == null || Nodes.Length == 0;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];

				if (match == null || match.Length == 0)
				{
					IsReturnPath = true;
					LookAheadMatch = Array.Empty<MatchEntry>();
				}
				else
				{
					Match = match[0];
					LookAheadMatch = match;
				}

				Weight = weight;

				CalcInfo(ref Weight, out PassLazyNode, out EnterReturnNodes, out StackEvalDelta);
			}

			public ExecutionPath(Node pathSourceNode, Node[] route, int weight, PredicateEntryBase predicate)
			{
				PathSourceNode = pathSourceNode;
				Nodes = route;
				IsInvalid = Nodes == null || Nodes.Length == 0;
				Output = IsInvalid ? null : Nodes[Nodes.Length -1];
				IsReturnPath = false;
				Predicate = predicate;
				LookAheadMatch = Array.Empty<MatchEntry>();
				Weight = weight;

				CalcInfo(ref Weight, out PassLazyNode, out EnterReturnNodes, out StackEvalDelta);
			}

			public static ExecutionPath JoinPaths(IReadOnlyList<ExecutionPath> executionPaths)
			{
				return new ExecutionPath(executionPaths[0].PathSourceNode, executionPaths.SelectMany(p => p.Nodes).ToArray(), executionPaths.Max(p => p.Weight), executionPaths.SelectMany(p => p.LookAheadMatch).Where(m => m != null).ToArray());
			}

			public ExecutionPath(IPool<ExecutionPath> forkExecutionPathPool)
			{
				_forkExecutionPathPool = forkExecutionPathPool;
				Nodes = new Node[1];
				IsInvalid = false;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];
				EnterReturnNodes = Array.Empty<Node>();
				ForkPredicatePath = true;
				LookAheadMatch = Array.Empty<MatchEntry>();
				Weight = 0;
				IsReturnPath = false;
			}

			private string DebuggerDisplay
			{
				get
				{
					if (IsInvalid)
						return "???";

					var operandStr = Match?.ToString() ?? "?";

					return SourceNode + " - {" + (IsReturnPath ? "Ret" : operandStr) + "} - " + Output;
				}
			}

			private string DebugView
			{
				get
				{
					return string.Join("\n", Nodes.Select(n => n.ToString()));
				}
			}

			public bool IsPredicate => Predicate != null;

			public int PriorityIndex { get; set; }

			private Node SourceNode => IsInvalid ? null : Nodes[0];

			public ExecutionPath AddReference()
			{
				ReferenceCount.AddReference();

				return this;
			}

			private void CalcInfo(ref int weight, out bool passLazyNode, out Node[] enterNodes, out int stackEvalDelta)
			{
				passLazyNode = false;
				stackEvalDelta = 0;

				if (Nodes == null || Nodes.Length == 0)
				{
					if (weight == -1)
						weight = 0;

					enterNodes = Array.Empty<Node>();

					return;
				}

				var flags = 0;
				List<Node> enterNodesList = null;

				if (weight == -1)
				{
					weight = 0;
					var firstNode = Nodes[0];
					var current = firstNode;

					foreach (var node in Nodes)
					{
						flags |= node.Flags;

						if (node is EnterRuleNode)
						{
							stackEvalDelta++;
							(enterNodesList ??= new List<Node>()).Add(node);
						}
						else if (node is ReturnRuleNode)
						{
							//stackEvalDelta--;
							(enterNodesList ??= new List<Node>()).Add(node);
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
						if (node is EnterRuleNode)
						{
							stackEvalDelta++;
							(enterNodesList ??= new List<Node>()).Add(node);
						}
						else if (node is ReturnRuleNode)
						{
							//stackEvalDelta--;
							(enterNodesList ??= new List<Node>()).Add(node);
						}

						flags |= node.Flags;
					}
				}

				enterNodes = enterNodesList != null ? enterNodesList.ToArray() : Array.Empty<Node>();

				passLazyNode = (flags & Node.Lazy) != 0;
			}

			private bool Equals(ExecutionPath other)
			{
				return Equals(Nodes, other.Nodes) && Equals(Match, other.Match);
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
					return ((Nodes != null ? Nodes.GetHashCode() : 0) * 397) ^ (Match != null ? Match.GetHashCode() : 0);
				}
			}

			public void MakeSafe()
			{
				if (Safe)
					return;

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

				ReferenceCount.AddReference();

				return this;
			}

			public void Release()
			{
				if (ForkPredicatePath == false)
					return;

				if (ReferenceCount.ReleaseReference() > 0)
					return;

				PathSourceNode = null;

				var predicateNode = (PredicateNode) Nodes[0];

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

			//public readonly DebugId<ExecutionPath> Id = DebugId<ExecutionPath>.Create("Exe");
		}
	}
}