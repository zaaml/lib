// <copyright file="Grammar.Lexer.FragmentSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class FragmentSymbol : PrimitiveSymbol
			{
				public FragmentSymbol(FragmentSyntax fragment)
				{
					Fragment = fragment;
				}

				public FragmentSyntax Fragment { get; }
			}

			protected internal sealed class TokenSymbol : PrimitiveSymbol
			{
				public TokenSymbol(TokenSyntax token)
				{
					Token = token;
				}

				public TokenSyntax Token { get; }
			}
		}
	}
}