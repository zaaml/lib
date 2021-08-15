// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserForkBranchPredicateResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private sealed class ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken> : PredicateResult
					where TExternalGrammar : Grammar<TExternalToken> where TExternalToken : unmanaged, Enum
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

				private sealed class ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> : PredicateResult<TExternalNode>
					where TExternalGrammar : Grammar<TExternalToken, TExternalNodeBase> where TExternalToken : unmanaged, Enum where TExternalNode : TExternalNodeBase where TExternalNodeBase : class
				{
					private readonly IPool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>> _pool;
					private ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> _forkBranch;

					public ExternalParserForkBranchPredicateResult(IPool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>> pool)
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

					public ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> Mount(ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> forkBranch)
					{
						_forkBranch = forkBranch;

						return this;
					}
				}
			}
		}
	}
}