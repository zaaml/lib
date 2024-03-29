// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private abstract class ExternalParserContext : IDisposable
				{
					private ReferenceCounter _referenceCount;

					protected void AddReferenceCore()
					{
						_referenceCount.AddReference();
					}

					protected void ReleaseReferenceCore()
					{
						if (_referenceCount.ReleaseReference() == 0)
							Dispose();
					}

					public abstract void Dispose();
				}

				private sealed class ExternalParserContext<TExternalGrammar, TExternalToken> : ExternalParserContext
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum
				{
					private readonly ExternalParserResources<TExternalGrammar, TExternalToken> _resources;

					public ExternalParserContext(ExternalParserResources<TExternalGrammar, TExternalToken> resources)
					{
						_resources = resources;
					}

					private Parser<TExternalGrammar, TExternalToken>.ParserAutomata.ProcessAutomataContext ExternalAutomataContext { get; set; }

					public ExternalParserDelegate<TExternalGrammar, TExternalToken> ExternalParserDelegate { get; private set; }

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
						ExternalAutomataContext = ExternalAutomataContext.DisposeExchange();
						ExternalResult = ExternalResult.DisposeExchange();

						_resources.ExternalParserContextPool.Return(this);
					}

					public ExternalParserContext<TExternalGrammar, TExternalToken> Mount(ParserAutomataContext parserAutomataContext, ExternalParserDelegate<TExternalGrammar, TExternalToken> externalParserDelegate,
						LexemeSource<TExternalToken> lexemeSource, int offset,
						Parser<TExternalGrammar, TExternalToken>.ParserAutomata.ProcessAutomataContext externalAutomataContext, Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult externalResult)
					{
						Offset = offset;
						LexemeSource = lexemeSource;
						ExternalResult = externalResult;
						ExternalParserDelegate = externalParserDelegate;
						InternalAutomataContext = parserAutomataContext;
						ExternalAutomataContext = externalAutomataContext;

						return AddReference();
					}

					public void ReleaseReference()
					{
						ReleaseReferenceCore();
					}
				}

				private sealed class ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode> : ExternalParserContext
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> 
					where TExternalToken : unmanaged, Enum 
					where TExternalNode : class
				{
					private readonly ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode> _resources;

					public ExternalParserContext(ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode> resources)
					{
						_resources = resources;
					}

					public Parser<TExternalGrammar, TExternalToken>.ParserAutomata.SyntaxTreeAutomataContext ExternalAutomataContext { get; private set; }

					public ExternalParserDelegate<TExternalGrammar, TExternalToken, TExternalNode> ExternalParserDelegate { get; private set; }

					private Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult ExternalResult { get; set; }

					public ParserAutomataContext InternalAutomataContext { get; private set; }

					private LexemeSource<TExternalToken> LexemeSource { get; set; }

					public int Offset { get; private set; }

					public ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode> AddReference()
					{
						AddReferenceCore();

						return this;
					}

					public override void Dispose()
					{
						LexemeSource = LexemeSource.DisposeExchange();
						ExternalAutomataContext = ExternalAutomataContext.DisposeExchange();
						ExternalResult = ExternalResult.DisposeExchange();
						_resources.ExternalParserContextPool.Return(this);
					}

					public ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode> Mount(ParserAutomataContext parserAutomataContext,
						ExternalParserDelegate<TExternalGrammar, TExternalToken, TExternalNode> externalParserDelegate,
						LexemeSource<TExternalToken> lexemeSource, int offset,
						Parser<TExternalGrammar, TExternalToken>.ParserAutomata.SyntaxTreeAutomataContext externalAutomataContext, Automata<Lexeme<TExternalToken>, TExternalToken>.AutomataResult externalResult)
					{
						Offset = offset;
						LexemeSource = lexemeSource;
						ExternalResult = externalResult;
						ExternalParserDelegate = externalParserDelegate;
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