// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserResources.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private abstract class ExternalParserResources
				{
				}

				private sealed class ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> : ExternalParserResources
					where TExternalGrammar : Grammar<TExternalToken, TExternalNodeBase> where TExternalToken : unmanaged, Enum where TExternalNode : TExternalNodeBase where TExternalNodeBase : class
				{
					public readonly Pool<ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>> ExternalParserContextPool;
					public readonly Pool<ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>> ExternalParserForkBranchPool;
					public readonly Pool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>> ExternalParserForkBranchPredicateResultPool;
					public readonly Pool<ExternalParserForkPredicateResult<TExternalNode>> ExternalParserForkPredicateResultPool;
					public readonly Pool<ExternalParserPredicateResult<TExternalNode>> ExternalParserPredicateResultPool;

					public ExternalParserResources()
					{
						ExternalParserForkBranchPredicateResultPool =
							new Pool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>>(p =>
								new ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>(p));
						ExternalParserForkPredicateResultPool = new Pool<ExternalParserForkPredicateResult<TExternalNode>>(p => new ExternalParserForkPredicateResult<TExternalNode>(p));
						ExternalParserPredicateResultPool = new Pool<ExternalParserPredicateResult<TExternalNode>>(p => new ExternalParserPredicateResult<TExternalNode>(p));
						ExternalParserForkBranchPool =
							new Pool<ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>>(p => new ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>(this));
						ExternalParserContextPool =
							new Pool<ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>>(p => new ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>(this));
					}
				}

				private sealed class ExternalParserResources<TExternalGrammar, TExternalToken> : ExternalParserResources
					where TExternalGrammar : Grammar<TExternalToken> where TExternalToken : unmanaged, Enum
				{
					public readonly Pool<ExternalParserContext<TExternalGrammar, TExternalToken>> ExternalParserContextPool;
					public readonly Pool<ExternalParserForkBranch<TExternalGrammar, TExternalToken>> ExternalParserForkBranchPool;
					public readonly Pool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken>> ExternalParserForkBranchPredicateResultPool;
					public readonly Pool<ExternalParserForkPredicateResult> ExternalParserForkPredicateResultPool;
					public readonly Pool<ExternalParserPredicateResult<object>> ExternalParserPredicateResultPool;

					public ExternalParserResources()
					{
						ExternalParserForkBranchPredicateResultPool = new Pool<ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken>>(p => new ExternalParserForkBranchPredicateResult<TExternalGrammar, TExternalToken>(p));
						ExternalParserForkPredicateResultPool = new Pool<ExternalParserForkPredicateResult>(p => new ExternalParserForkPredicateResult(p));
						ExternalParserPredicateResultPool = new Pool<ExternalParserPredicateResult<object>>(p => new ExternalParserPredicateResult<object>(p));
						ExternalParserForkBranchPool = new Pool<ExternalParserForkBranch<TExternalGrammar, TExternalToken>>(p => new ExternalParserForkBranch<TExternalGrammar, TExternalToken>(this));
						ExternalParserContextPool = new Pool<ExternalParserContext<TExternalGrammar, TExternalToken>>(p => new ExternalParserContext<TExternalGrammar, TExternalToken>(this));
					}
				}
			}
		}
	}
}