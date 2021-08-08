// <copyright file="PredicateNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class PredicateNode : Node
		{
			private readonly IPool<PredicateNode> _pool;

			public PredicateNode(Automata<TInstruction, TOperand> automata, Graph graph, PredicateEntryBase predicateEntry) : base(automata, graph)
			{
				PredicateEntry = predicateEntry;
			}

			public PredicateNode(Automata<TInstruction, TOperand> automata, IPool<PredicateNode> pool) : base(automata, null)
			{
				_pool = pool;
			}

			public int ForkPathIndex { get; set; } = -1;

			protected override string KindString => "_predicate";

			[UsedImplicitly]
			public PredicateEntryBase PredicateEntry { get; private set; }

			public PredicateNode Mount(PredicateEntryBase predicateEntry)
			{
				PredicateEntry = predicateEntry;

				return this;
			}

			public void Release()
			{
				_pool.Release(this);
			}
		}
	}
}