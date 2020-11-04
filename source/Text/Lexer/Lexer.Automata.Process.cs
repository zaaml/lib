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

				protected LexerProcess(TextSource textSource)
				{
					TextSource = textSource;
				}

				#endregion

				#region Properties

				public abstract int TextPointer { get; set; }

				public TextSource TextSource { get; }

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