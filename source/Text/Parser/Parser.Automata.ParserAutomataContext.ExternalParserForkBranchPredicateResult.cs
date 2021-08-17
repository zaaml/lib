// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserForkBranchPredicateResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private sealed class ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken> : PredicateResult
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum
				{
					private readonly IPool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken>> _pool;

					private ExternalParserForkBranch<TExternalGrammar, TExternalToken> _forkBranch;

					public ExternalParserForkBranchPredicateResult(IPool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken>> pool)
					{
						_pool = pool;
					}

					internal override void Dispose()
					{
						base.Dispose();

						_forkBranch = null;

						_pool.Release(this);
					}

					public ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken> Mount(ExternalParserForkBranch<TExternalGrammar, TExternalToken> branch)
					{
						_forkBranch = branch;

						return this;
					}
				}

				private sealed class ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode> : PredicateResult<TExternalNode>
					where TExternalGrammar : Grammar<TExternalGrammar,TExternalToken> 
					where TExternalToken : unmanaged, Enum 
					where TExternalNode : class
				{
					private readonly IPool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode>> _pool;
					private ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode> _forkBranch;

					public ExternalParserForkBranchPredicateResult(IPool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode>> pool)
					{
						_pool = pool;
					}

					internal override void Dispose()
					{
						base.Dispose();

						_forkBranch = null;
						_pool.Release(this);
					}

					protected override TExternalNode GetResultCore()
					{
						if (_forkBranch.Finish)
							_forkBranch.ExternalResult.RunFirst().Dispose();

						return _forkBranch.ExternalContext.ExternalAutomataContext.GetResult<TExternalNode>();
					}

					public ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode> Mount(ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode> forkBranch)
					{
						_forkBranch = forkBranch;

						return this;
					}
				}
			}
		}
	}
}