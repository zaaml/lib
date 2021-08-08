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
				private readonly Stack<LexerAutomataContextState> _lexerAutomataContextStatesPool = new();
				private LexerContext<TToken> _lexerContext;
				private TextSpan _textSourceSpan;

				public LexerAutomataContext(LexerAutomata automata) : base(null, automata)
				{
				}

				public LexerContext<TToken> LexerContext => _lexerContext;

				public override Process Process => null;

				protected override AutomataContextState CreateContextState()
				{
					if (_lexerContext == null)
						return null;

					var contextState = _lexerAutomataContextStatesPool.Count > 0 ? _lexerAutomataContextStatesPool.Pop() : new LexerAutomataContextState();

					contextState.LexerContext = _lexerContext;

					return contextState;
				}

				public override ProcessKind ProcessKind => ProcessKind.Process;

				public void Mount(TextSpan textSourceSpan, LexerContext<TToken> parserContext)
				{
					_textSourceSpan = textSourceSpan;
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