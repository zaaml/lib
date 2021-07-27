// <copyright file="Lexer.Automata.Process.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		#region Nested Types

		private protected partial class LexerAutomata
		{
			#region Nested Types

			public abstract class LexerProcess : IDisposable
			{
				#region Ctors

				protected LexerProcess(TextSpan textSourceSpan)
				{
					TextSourceSpan = textSourceSpan;
				}

				#endregion

				#region Properties

				public abstract int TextPointer { get; set; }

				public TextSpan TextSourceSpan { get; }

				#endregion

				#region Methods

				public abstract int Run(Lexeme<TToken>[] lexemes, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes);

				#endregion

				#region Interface Implementations

				#region IDisposable

				public abstract void Dispose();

				#endregion

				#endregion
			}

			#endregion
		}

		#endregion
	}
}