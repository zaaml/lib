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
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly Dictionary<ExecutionPathKey, Node[]> _executionRouteDictionary = new();
		private readonly List<ExecutionPath> _executionPathRegistry = new();

		private void RegisterExecutionPath(ExecutionPath executionPath)
		{
			lock (_executionPathRegistry)
			{
				executionPath.Id = _executionPathRegistry.Count;

				_executionPathRegistry.Add(executionPath);

				foreach (var executionMethodBuilder in _executionMethodBuilders) 
					executionPath.BuildExecutionMethods(executionMethodBuilder);

				if (ExecutionPathLookAheadLength < executionPath.LookAheadMatch.Length)
					ExecutionPathLookAheadLength = executionPath.LookAheadMatch.Length;
			}
		}

		private ExecutionPath CreateExecutionPath(Node pathSourceNode, Node[] route, PredicateEntryBase predicate)
		{
			lock (_executionPathRegistry)
			{
				var key = new ExecutionPathKey(pathSourceNode, route, predicate);

				if (_executionRouteDictionary.TryGetValue(key, out _) == false) 
					_executionRouteDictionary.Add(key, route);

				var executionPath = new ExecutionPath(pathSourceNode, route, predicate);

				RegisterExecutionPath(executionPath);

				return executionPath;
			}
		}

		private ExecutionPath CreateExecutionPath(Node pathSourceNode, Node[] route, params MatchEntry[] match)
		{
			lock (_executionPathRegistry)
			{
				var key = new ExecutionPathKey(pathSourceNode, route, match);

				if (_executionRouteDictionary.TryGetValue(key, out _) == false)
					_executionRouteDictionary.Add(key, route);

				var executionPath = new ExecutionPath(pathSourceNode, route, match);

				RegisterExecutionPath(executionPath);

				return executionPath;
			}
		}

		private sealed class ExecutionPathKey
		{
			private readonly int _hashCode;
			private readonly MatchEntry[] _match;
			private readonly Node _pathSourceNode;
			private readonly PredicateEntryBase _predicate;
			private readonly Node[] _route;

			public ExecutionPathKey(Node pathSourceNode, Node[] route, PredicateEntryBase predicate)
			{
				_pathSourceNode = pathSourceNode;
				_route = route;
				_predicate = predicate;
				_hashCode = CalcHashCode();
			}

			public ExecutionPathKey(Node pathSourceNode, Node[] route, params MatchEntry[] match)
			{
				_pathSourceNode = pathSourceNode;
				_route = route;
				_match = match;
				_hashCode = CalcHashCode();
			}

			private bool Equals(ExecutionPathKey other)
			{
				//if (Equals(_pathSourceNode, other._pathSourceNode) == false)
				//	return false;

				if (_route.SequenceEqual(other._route) == false)
					return false;

				//if (Equals(_predicate, other._predicate) == false)
				//	return false;

				//if (_match == null || other._match == null)
				//	return _match == null && other._match == null;

				//return _match.SequenceEqual(other._match);

				return true;
			}

			public override bool Equals(object obj)
			{
				return ReferenceEquals(this, obj) || obj is ExecutionPathKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}

			private int CalcHashCode()
			{
				unchecked
				{
					var hashCode = 0;

					//hashCode = _pathSourceNode?.GetHashCode() ?? 0;

					foreach (var node in _route)
						hashCode = (hashCode * 397) ^ node.GetHashCode();

					//hashCode = (hashCode * 397) ^ (_predicate?.GetHashCode() ?? 0);

					//if (_match != null)
					//{
					//	foreach (var matchEntry in _match)
					//		hashCode = (hashCode * 397) ^ matchEntry.GetHashCode();
					//}

					return hashCode;
				}
			}
		}

		private protected abstract class ExecutionPathBase
		{
		}

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private protected sealed class ExecutionPath : ExecutionPathBase
		{
			public static readonly ExecutionPath Invalid = new(null, Array.Empty<Node>());

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
			private ExecutionPathMethodDelegate[] _executionMethods = Array.Empty<ExecutionPathMethodDelegate>();
			public int Id;
			public ExecutionPath LookAheadPath;
			public Node Output;
			public Node PathSourceNode;
			public PredicateEntryBase Predicate;
			public ReferenceCounter ReferenceCount;
			public bool Safe;

			public ExecutionPath(Node pathSourceNode, Node[] route, params MatchEntry[] match)
			{
				PathSourceNode = pathSourceNode;
				Nodes = route;
				IsInvalid = Nodes == null || Nodes.Length == 0;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];
				OutputReturn = Output is ReturnSyntaxNode;
				OutputEnd = Output is ExitSyntaxNode;

				if (match == null || match.Length == 0)
					LookAheadMatch = Array.Empty<MatchEntry>();
				else
				{
					Match = match[0];
					LookAheadMatch = match;
				}

				CalcInfo(out PassLazyNode, out EnterReturnSubGraphs, out PrecedenceNodes, out HasPrecedenceNodes, out StackDepth, out StackDelta, out ContextEval);
			}

			public ExecutionPath(Node pathSourceNode, Node[] route, PredicateEntryBase predicate)
			{
				PathSourceNode = pathSourceNode;
				Nodes = route;
				IsInvalid = Nodes == null || Nodes.Length == 0;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];

				Debug.Assert(Output is PredicateNode);

				OutputReturn = false;
				Predicate = predicate;
				LookAheadMatch = Array.Empty<MatchEntry>();

				CalcInfo(out PassLazyNode, out EnterReturnSubGraphs, out PrecedenceNodes, out HasPrecedenceNodes, out StackDepth, out StackDelta, out ContextEval);
			}

			public ExecutionPath(Automata<TInstruction, TOperand> automata, IPool<ExecutionPath> forkExecutionPathPool)
			{
				_forkExecutionPathPool = forkExecutionPathPool;

				Nodes = new Node[1];
				IsInvalid = false;
				Output = IsInvalid ? null : Nodes[Nodes.Length - 1];
				OutputReturn = Output is ReturnSyntaxNode;
				OutputEnd = Output is ExitSyntaxNode;
				EnterReturnSubGraphs = Array.Empty<int>();
				PrecedenceNodes = Array.Empty<PrecedenceNode>();
				ForkPredicatePath = true;
				ContextEval = false;
				LookAheadMatch = Array.Empty<MatchEntry>();

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

			public bool IsEpsilon => OutputReturn || OutputEnd || IsPredicate;

			public int PriorityIndex { get; set; }

			private Node SourceNode => IsInvalid ? null : Nodes[0];

			public ExecutionPath AddReference()
			{
				ReferenceCount.AddReference();

				return this;
			}

			private void CalcInfo(out bool passLazyNode, out int[] enterReturnSubGraphs, out PrecedenceNode[] precedenceNodes, out bool hasPrecedenceNodes, out int stackDepth, out int stackDelta, out bool contextEval)
			{
				contextEval = Predicate == null;
				passLazyNode = false;
				stackDepth = 0;
				stackDelta = 0;

				if (Nodes == null || Nodes.Length == 0)
				{
					enterReturnSubGraphs = Array.Empty<int>();
					precedenceNodes = Array.Empty<PrecedenceNode>();
					hasPrecedenceNodes = false;

					return;
				}

				var flags = 0;
				List<int> enterReturnSubGraphList = null;
				List<PrecedenceNode> precedenceNodeList = null;

				foreach (var node in Nodes)
				{
					if (node is EnterSyntaxNode enterRuleNode)
					{
						stackDepth++;
						stackDelta++;

						var rid = enterRuleNode.SubGraph.RId == -1 ? enterRuleNode.SubGraph.Id : enterRuleNode.SubGraph.RId;

						(enterReturnSubGraphList ??= new List<int>()).Add(rid);
					}
					else if (node is ReturnSyntaxNode)
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
						if (node is EnterSyntaxNode enterStateNode)
						{
							var subGraph = enterStateNode.SubGraph;
							var leaveStateNode = subGraph.LeaveNode;

							if (leaveStateNode.Safe == false)
								leaveStateNode.MakeSafe();
						}
					}

					if (Output is LeaveSyntaxNode == false)
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

				_forkExecutionPathPool.Return(this);
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node Execute(int index, Process process)
			{
				return _executionMethods[index](this, process);
			}

			internal void BindExecutionMethod(int index, ExecutionPathMethodDelegate method)
			{
				_executionMethods[index] = method;
			}

			internal void BuildExecutionMethods(ExecutionMethodBuilder executionMethodBuilder)
			{
				var capacity = BitUtils.Power2Ceiling(Math.Max(2, executionMethodBuilder.BuilderIndex * 2));

				if (_executionMethods.Length < capacity)
				{
					var executionMethods = new ExecutionPathMethodDelegate[capacity];

					for (var i = 0; i < _executionMethods.Length; i++) 
						executionMethods[i] = _executionMethods[i];

					_executionMethods = executionMethods;
				}

				executionMethodBuilder.BuildExecutionMethods(this);
			}
		}
	}
}