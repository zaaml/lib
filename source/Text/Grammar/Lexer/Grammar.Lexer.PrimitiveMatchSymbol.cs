// <copyright file="Grammar.Lexer.PrimitiveMatchSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal abstract class PrimitiveMatchSymbol : MatchSymbol
			{
				public static implicit operator PrimitiveMatchSymbol(char c)
				{
					return new CharSymbol(c);
				}

				public static implicit operator PrimitiveMatchSymbol(int c)
				{
					return new CharSymbol((char)c);
				}

#if NETCOREAPP3_1 || NET5_0_OR_GREATER
				public static implicit operator PrimitiveMatchSymbol(Range range)
				{
					return new CharRangeSymbol((char)range.Start.Value, (char)range.End.Value);
				}
#endif
			}
		}
	}
}