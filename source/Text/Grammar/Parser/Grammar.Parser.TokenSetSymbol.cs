// <copyright file="Grammar.Parser.TokenSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class TokenSetSymbol : PrimitiveSymbol
			{
				public TokenSetSymbol(LexerGrammar.TokenSyntax[] tokens)
				{
					Tokens = tokens;
				}

				public LexerGrammar.TokenSyntax[] Tokens { get; }

				public TokenSetSymbol Bind(string name)
				{
					ArgumentName = name;

					return this;
				}
			}
		}
	}
}