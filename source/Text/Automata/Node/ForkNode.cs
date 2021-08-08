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

			public PredicateEntryBase ActualEntry => ((PredicateNode)ExecutionPath.Nodes[NodeIndex]).PredicateEntry.GetActualPredicateEntry();

			public ExecutionPath ExecutionPath { get; private set; }

			protected override string KindString => "_fork";

			public int NodeIndex { get; private set; }

			public PredicateResult PredicateResult { get; private set; }

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
		}
	}
}