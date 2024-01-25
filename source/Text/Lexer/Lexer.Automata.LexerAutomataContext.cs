// <copyright file="Lexer.Automata.LexerAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerAutomataContext : AutomataContext, ILexerContext, IDisposable
			{
				public LexerAutomataContext(Lexer<TGrammar, TToken> lexer, TextSpan text, LexerAutomata automata) 
					: base(null, automata, lexer.ServiceProvider)
				{
					Lexer = lexer;
					Text = text;

					Lexer.AttachContext(this);
				}

				public Lexer<TGrammar, TToken> Lexer { get; }

				public TextSpan Text { get; }

				public override Process Process => null;

				public override ProcessKind ProcessKind => ProcessKind.Process;

				public int Position { get; set; }

				public void Dispose()
				{
					Lexer.DetachContext(this);
				}

				TextPoint ILexerContext.Position
				{
					get => Text.At(Position);
					set => Position = value.Index;
				}

				TextSpan ILexerContext.Text => Text;
			}
		}
	}
}