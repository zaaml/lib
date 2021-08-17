﻿// <copyright file="Grammar.Lexer.CharRangeSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class CharRangeSymbol : PrimitiveMatchSymbol
			{
				public CharRangeSymbol(char first, char last)
				{
					First = first;
					Last = last;
				}

				public char First { get; }

				public char Last { get; }
			}
		}
	}
}