// <copyright file="Grammar.Parser.TokenSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class TokenSymbol : PrimitiveSymbol
			{
				public TokenSymbol(LexerGrammar.TokenSyntax token)
				{
					Token = token;
				}

				public LexerGrammar.TokenSyntax Token { get; }

				public TokenSymbol Bind(string name)
				{
					ArgumentName = name;

					return this;
				}
			}
		}
	}
}