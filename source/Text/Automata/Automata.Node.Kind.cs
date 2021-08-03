﻿// <copyright file="Automata.Node.Kind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private sealed class BeginRuleNode : Node
		{
			#region Ctors

			public BeginRuleNode(Automata<TInstruction, TOperand> automata, Graph graph) : base(automata, graph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => "_begin";

			#endregion
		}

		private sealed class ReturnRuleNode : Node
		{
			#region Ctors

			public ReturnRuleNode(Automata<TInstruction, TOperand> automata, Graph graph) : base(automata, graph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => "_return";

			#endregion
		}

		private abstract class SubGraphNode : Node
		{
			#region Fields

			public readonly SubGraph SubGraph;

			#endregion

			#region Ctors

			protected SubGraphNode(Automata<TInstruction, TOperand> automata, Graph graph, SubGraph subGraph) : base(automata, graph)
			{
				SubGraph = subGraph;
			}

			#endregion
		}

		private abstract class EntryPointSubGraphNode : SubGraphNode
		{
			#region Ctors

			protected EntryPointSubGraphNode(Automata<TInstruction, TOperand> automata, SubGraph subGraph) : base(automata, null, subGraph)
			{
			}

			#endregion
		}

		private sealed class EnterRuleNode : SubGraphNode
		{
			#region Ctors

			public EnterRuleNode(Automata<TInstruction, TOperand> automata, Graph graph, SubGraph subGraph) : base(automata, graph, subGraph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_enter({SubGraph})";

			#endregion
		}

		private sealed class LeaveRuleNode : SubGraphNode
		{
			#region Ctors

			public LeaveRuleNode(Automata<TInstruction, TOperand> automata, Graph graph, SubGraph subGraph) : base(automata, graph, subGraph)
			{
				//LeavePath = new ExecutionPath(this, new Node[] {this});
			}

			#endregion

			#region Properties

			protected override string KindString => $"_leave({SubGraph})";

			#endregion

			//private ExecutionPath LeavePath { get; }
		}

		private abstract class RuleEntryNode : Node
		{
			#region Fields

			public readonly RuleEntry RuleEntry;

			#endregion

			#region Ctors

			protected RuleEntryNode(Automata<TInstruction, TOperand> automata, Graph graph, RuleEntry ruleEntry) : base(automata, graph)
			{
				RuleEntry = ruleEntry;
			}

			#endregion
		}

		private sealed class InlineEnterRuleNode : RuleEntryNode
		{
			#region Ctors

			public InlineEnterRuleNode(Automata<TInstruction, TOperand> automata, Graph graph, RuleEntry ruleEntry) : base(automata, graph, ruleEntry)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_inline_enter({RuleEntry})";

			#endregion
		}

		private sealed class InlineLeaveRuleNode : RuleEntryNode
		{
			#region Ctors

			public InlineLeaveRuleNode(Automata<TInstruction, TOperand> automata, Graph graph, RuleEntry ruleEntry) : base(automata, graph, ruleEntry)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_inline_leave({RuleEntry})";

			#endregion
		}

		private sealed class InitRuleNode : EntryPointSubGraphNode
		{
			#region Ctors

			public InitRuleNode(Automata<TInstruction, TOperand> automata, SubGraph subGraph) : base(automata, subGraph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_init({SubGraph})";

			#endregion
		}

		private sealed class EndRuleNode : EntryPointSubGraphNode
		{
			#region Ctors

			public EndRuleNode(Automata<TInstruction, TOperand> automata, SubGraph subGraph) : base(automata, subGraph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_end({SubGraph})";

			#endregion
		}

		private sealed class InnerNode : Node
		{
			#region Ctors

			public InnerNode(Automata<TInstruction, TOperand> automata, Graph graph) : base(automata, graph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_inner_{Index}";

			#endregion
		}

		private sealed class LazyNode : Node
		{
			#region Ctors

			public LazyNode(Automata<TInstruction, TOperand> automata, Graph graph) : base(automata, graph)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => "_lazy";

			#endregion
		}

		private sealed class PredicateNode : Node
		{
			#region Fields

			private readonly IPool<PredicateNode> _pool;

			#endregion

			#region Ctors

			public PredicateNode(Automata<TInstruction, TOperand> automata, Graph graph, PredicateEntryBase predicateEntry) : base(automata, graph)
			{
				PredicateEntry = predicateEntry;
			}

			public PredicateNode(Automata<TInstruction, TOperand> automata, IPool<PredicateNode> pool) : base(automata, null)
			{
				_pool = pool;
			}

			#endregion

			#region Properties

			public int ForkPathIndex { get; set; } = -1;

			protected override string KindString => "_predicate";

			[UsedImplicitly] public PredicateEntryBase PredicateEntry { get; private set; }

			#endregion

			#region Methods

			public PredicateNode Mount(PredicateEntryBase predicateEntry)
			{
				PredicateEntry = predicateEntry;

				return this;
			}

			public void Release()
			{
				_pool.Release(this);
			}

			#endregion
		}

		private sealed class ActionNode : Node
		{
			#region Ctors

			public ActionNode(Automata<TInstruction, TOperand> automata, Graph graph, ActionEntry actionEntry) : base(automata, graph)
			{
				ActionEntry = actionEntry;
			}

			#endregion

			#region Properties

			[UsedImplicitly] public ActionEntry ActionEntry { get; }

			protected override string KindString => "_action";

			#endregion
		}

		private abstract class ProductionNode : Node
		{
			#region Ctors

			protected ProductionNode(Automata<TInstruction, TOperand> automata, Graph graph, Production production) : base(automata, graph)
			{
				Production = production;
			}

			#endregion

			#region Properties

			public Production Production { get; }

			#endregion
		}

		private sealed class BeginProductionNode : ProductionNode
		{
			#region Ctors

			public BeginProductionNode(Automata<TInstruction, TOperand> automata, Graph graph, Production production) : base(automata, graph, production)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => $"_begin({Production})";

			#endregion
		}

		private sealed class EndProductionNode : ProductionNode
		{
			#region Ctors

			public EndProductionNode(Automata<TInstruction, TOperand> automata, Graph graph, Production production) : base(automata, graph, production)
			{
			}

			#endregion

			#region Properties

			protected override string KindString => "_endProduction";

			#endregion
		}

		private sealed class OperandNode : Node
		{
			#region Ctors

			public OperandNode(Automata<TInstruction, TOperand> automata, Graph graph, MatchEntry matchEntry) : base(automata, graph)
			{
				MatchEntry = matchEntry;
			}

			#endregion

			#region Properties

			protected override string KindString => $"_operand({MatchEntry})";

			public MatchEntry MatchEntry { get; }

			#endregion
		}

		private sealed class ForkNode : Node
		{
			#region Fields

			private readonly IPool<ForkNode> _pool;

			#endregion

			#region Ctors

			public ForkNode(Automata<TInstruction, TOperand> automata, IPool<ForkNode> pool) : base(automata, null)
			{
				_pool = pool;
			}

			#endregion

			#region Properties

			public PredicateEntryBase ActualEntry => ((PredicateNode) ExecutionPath.Nodes[NodeIndex]).PredicateEntry.GetActualPredicateEntry();

			public ExecutionPath ExecutionPath { get; private set; }

			public List<ExecutionPath> ForkExecutionPaths { get; } = new List<ExecutionPath>();

			protected override string KindString => "_fork";

			public int NodeIndex { get; private set; }

			public PredicateResult PredicateResult { get; private set; }

			#endregion

			#region Methods

			public ForkNode Mount(int nodeIndex, ExecutionPath executionPath, PredicateResult predicateResult)
			{
				NodeIndex = nodeIndex;
				ExecutionPath = executionPath;
				PredicateResult = predicateResult;

				var sourceNode = ExecutionPath.Nodes[NodeIndex];

				CopyLookup(sourceNode);

				return this;
			}

			public void Release()
			{
				_pool.Release(this);
			}

			#endregion
		}

		#endregion
	}
}