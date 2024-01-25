// <copyright file="Lexer.Automata.Process.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			public abstract class LexerProcess : IDisposable
			{
				protected LexerProcess(TextSpan textSourceSpan)
				{
					TextSourceSpan = textSourceSpan;
				}

				public TextSpan TextSourceSpan { get; }

				public abstract int Run(ref int textPointer, Lexeme<TToken>[] lexemes, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes);

				public abstract void Dispose();
			}
		}
	}
}