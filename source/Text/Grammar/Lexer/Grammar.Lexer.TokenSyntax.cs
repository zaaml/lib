// <copyright file="Grammar.Lexer.TokenSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class TokenSyntax : TokenBaseSyntax
			{
				public TokenSyntax([CallerMemberName] string name = null) : base(name)
				{
					RegisterTokenSyntax(this);
				}

				public TokenSyntax AddProduction(TToken token, Production production)
				{
					AddProductionCore(token, production);

					return this;
				}
			}
		}
	}
}