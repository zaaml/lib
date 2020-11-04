// <copyright file="Lexer.Automata.LexerAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerAutomataContext : AutomataContext, ILexerAutomataContextInterface, IDisposable
			{
				private readonly Stack<LexerAutomataContextState> _lexerAutomataContextStatesPool = new Stack<LexerAutomataContextState>();
				private LexerContext<TToken> _lexerContext;
				private TextSource _textSource;

				public LexerAutomataContext() : base(null)
				{
				}

				public LexerContext<TToken> LexerContext => _lexerContext;

				protected override AutomataContextState CreateContextState()
				{
					if (_lexerContext == null)
						return null;

					var contextState = _lexerAutomataContextStatesPool.Count > 0 ? _lexerAutomataContextStatesPool.Pop() : new LexerAutomataContextState();

					contextState.LexerContext = _lexerContext;

					return contextState;
				}

				public void Mount(TextSource textSource, LexerContext<TToken> parserContext)
				{
					_textSource = textSource;
					_lexerContext = parserContext;

					if (_lexerContext != null)
						_lexerContext.LexerAutomataContext = this;
				}

				public void Dispose()
				{
				}
			}
		}
	}
}