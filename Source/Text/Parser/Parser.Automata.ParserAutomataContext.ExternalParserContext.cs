// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private abstract class ExternalParserContext : IDisposable
				{
					private ReferenceCounter ReferenceCount { get; }

					protected void AddReferenceCore()
					{
						ReferenceCount.AddReference();
					}

					protected void ReleaseReferenceCore()
					{
						if (ReferenceCount.ReleaseReference() == 0)
							Dispose();
					}

					public abstract void Dispose();
				}

				private sealed class ExternalParserContext<TExternalGrammar, TExternalToken> : ExternalParserContext
					where TExternalGrammar : Grammar<TExternalToken> where TExternalToken : unmanaged, Enum
				{
					private readonly ExternalParserResources<TExternalGrammar, TExternalToken> _resources;

					public ExternalParserContext(ExternalParserResources<TExternalGrammar, TExternalToken> resources)
					{
						_resources = resources;
					}

					private Parser<TExternalGrammar, TExternalToken>.ParserAutomata.ProcessAutomataContext ExternalAutomataContext { get; set; }

					private ParserContext ExternalContext { get; set; }

					public ExternalParserInvokeInfo<TExternalGrammar, TExternalToken> ExternalParserInvokeInfo { get; private set; }

					private Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult ExternalResult { get; set; }

					public ParserAutomataContext InternalAutomataContext { get; private set; }

					private LexemeSource<TExternalToken> LexemeSource { get; set; }

					public int Offset { get; private set; }

					public ExternalParserContext<TExternalGrammar, TExternalToken> AddReference()
					{
						AddReferenceCore();

						return this;
					}

					public override void Dispose()
					{
						LexemeSource = LexemeSource.DisposeExchange();
						ExternalContext = ExternalContext.DisposeExchange();
						ExternalAutomataContext = ExternalAutomataContext.DisposeExchange();
						ExternalResult = ExternalResult.DisposeExchange();

						_resources.ExternalParserContextPool.Release(this);
					}

					public ExternalParserContext<TExternalGrammar, TExternalToken> Mount(ParserAutomataContext parserAutomataContext, ExternalParserInvokeInfo<TExternalGrammar, TExternalToken> externalParserInvokeInfo,
						LexemeSource<TExternalToken> lexemeSource, int offset,
						ParserContext externalContext, Parser<TExternalGrammar, TExternalToken>.ParserAutomata.ProcessAutomataContext externalAutomataContext, Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult externalResult)
					{
						Offset = offset;
						LexemeSource = lexemeSource;
						ExternalResult = externalResult;
						ExternalContext = externalContext;
						ExternalParserInvokeInfo = externalParserInvokeInfo;
						InternalAutomataContext = parserAutomataContext;
						ExternalAutomataContext = externalAutomataContext;

						return AddReference();
					}

					public void ReleaseReference()
					{
						ReleaseReferenceCore();
					}
				}

				private sealed class ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> : ExternalParserContext
					where TExternalGrammar : Grammar<TExternalToken, TExternalNodeBase> where TExternalToken : unmanaged, Enum where TExternalNode : TExternalNodeBase where TExternalNodeBase : class
				{
					private readonly ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> _resources;

					public ExternalParserContext(ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> resources)
					{
						_resources = resources;
					}

					public Parser<TExternalGrammar, TExternalToken>.ParserAutomata.SyntaxTreeAutomataContext ExternalAutomataContext { get; private set; }

					private ParserContext ExternalContext { get; set; }

					public ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> ExternalParserInvokeInfo { get; private set; }

					private Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult ExternalResult { get; set; }

					public ParserAutomataContext InternalAutomataContext { get; private set; }

					private LexemeSource<TExternalToken> LexemeSource { get; set; }

					public int Offset { get; private set; }

					public ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> AddReference()
					{
						AddReferenceCore();

						return this;
					}

					public override void Dispose()
					{
						LexemeSource = LexemeSource.DisposeExchange();
						ExternalContext = ExternalContext.DisposeExchange();
						ExternalAutomataContext = ExternalAutomataContext.DisposeExchange();
						ExternalResult = ExternalResult.DisposeExchange();
						_resources.ExternalParserContextPool.Release(this);
					}

					public ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> Mount(ParserAutomataContext parserAutomataContext,
						ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> externalParserInvokeInfo,
						LexemeSource<TExternalToken> lexemeSource, int offset,
						ParserContext externalContext,
						Parser<TExternalGrammar, TExternalToken>.ParserAutomata.SyntaxTreeAutomataContext externalAutomataContext, Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult externalResult)
					{
						Offset = offset;
						LexemeSource = lexemeSource;
						ExternalResult = externalResult;
						ExternalContext = externalContext;
						ExternalParserInvokeInfo = externalParserInvokeInfo;
						InternalAutomataContext = parserAutomataContext;
						ExternalAutomataContext = externalAutomataContext;

						return AddReference();
					}

					public void ReleaseReference()
					{
						ReleaseReferenceCore();
					}
				}
			}
		}
	}
}