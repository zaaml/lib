// <copyright file="LexerExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text.Extensions
{
	internal static class LexerExtensions
	{
		#region Methods

		public static IEnumerable<Lexeme<TToken>> AsLexemeEnumerable<TGrammar, TToken>(this string str, Lexer<TGrammar, TToken> lexer) 
			where TToken : unmanaged, Enum 
			where TGrammar : Grammar<TGrammar, TToken>
		{
			return lexer.GetLexemeSource(new StringTextSource(str).GetTextSpan());
		}

		#endregion
	}
}