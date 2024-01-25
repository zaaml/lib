// <copyright file="Grammar.Lexer.CharSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class CharSymbol : PrimitiveMatchSymbol
			{
				public CharSymbol(int c)
				{
					Char = c;
				}

				public int Char { get; }
			}
		}
	}
}