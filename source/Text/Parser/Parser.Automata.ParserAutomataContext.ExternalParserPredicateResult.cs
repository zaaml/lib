// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserPredicateResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				protected sealed class ExternalParserPredicateResult<TResult> : PredicateResult<TResult>
				{
					private readonly IPool<ExternalParserPredicateResult<TResult>> _pool;

					private TResult _result;

					public ExternalParserPredicateResult(IPool<ExternalParserPredicateResult<TResult>> pool)
					{
						_pool = pool;
					}

					internal override void Dispose()
					{
						base.Dispose();

						_result = default;

						_pool.Return(this);
					}

					protected override TResult GetResultCore()
					{
						return _result;
					}

					public ExternalParserPredicateResult<TResult> Mount(TResult result)
					{
						_result = result;

						return this;
					}
				}
			}
		}
	}
}