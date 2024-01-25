// <copyright file="ForkNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ForkNode : Node
		{
			private readonly IPool<ForkNode> _pool;

			public ForkNode(Automata<TInstruction, TOperand> automata, IPool<ForkNode> pool) : base(automata, null, ThreadStatusKind.Fork)
			{
				_pool = pool;
			}

			public PredicateEntryBase ActualEntry => PredicateNode.PredicateEntry.GetActualPredicateEntry();

			protected override string KindString => "_fork";

			public PredicateResult PredicateResult { get; private set; }

			public PredicateNode PredicateNode { get; private set; }

			public ForkNode Mount(int nodeIndex, ExecutionPath executionPath, PredicateResult predicateResult)
			{
				PredicateResult = predicateResult;
				PredicateNode = (PredicateNode)executionPath.Nodes[nodeIndex];

				CopyLookup(PredicateNode);

				return this;
			}

			public ForkNode Mount(PredicateNode predicateNode, PredicateResult predicateResult)
			{
				PredicateResult = predicateResult;
				PredicateNode = predicateNode;

				CopyLookup(PredicateNode);

				return this;
			}

			public void Release()
			{
				_pool.Return(this);
			}
		}
	}
}