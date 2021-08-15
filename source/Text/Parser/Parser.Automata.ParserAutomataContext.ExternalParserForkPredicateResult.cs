// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserForkPredicateResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private sealed class ExternalParserForkPredicateResult : ForkPredicateResult
				{
					private readonly IPool<ExternalParserForkPredicateResult> _pool;

					public ExternalParserForkPredicateResult(IPool<ExternalParserForkPredicateResult> pool)
					{
						_pool = pool;
					}

					internal override void Dispose()
					{
						base.Dispose();

						_pool.Release(this);
					}

					public ExternalParserForkPredicateResult Mount(PredicateEntryBase first, PredicateEntryBase second)
					{
						First = first;
						Second = second;

						return this;
					}
				}

				private sealed class ExternalParserForkPredicateResult<TResult> : ForkPredicateResult<TResult>
				{
					private readonly IPool<ExternalParserForkPredicateResult<TResult>> _pool;

					public ExternalParserForkPredicateResult(IPool<ExternalParserForkPredicateResult<TResult>> pool)
					{
						_pool = pool;
					}

					internal override void Dispose()
					{
						base.Dispose();

						_pool.Release(this);
					}

					public ExternalParserForkPredicateResult<TResult> Mount(PredicateEntryBase first, PredicateEntryBase second)
					{
						First = first;
						Second = second;

						return this;
					}
				}
			}
		}
	}
}