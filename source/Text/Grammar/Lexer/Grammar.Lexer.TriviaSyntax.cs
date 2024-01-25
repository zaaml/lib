// <copyright file="Grammar.Lexer.TriviaSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class TriviaSyntax : TokenBaseSyntax
			{
				public TriviaSyntax([CallerMemberName] string name = null) : base(name)
				{
					RegisterTriviaSyntax(this);
				}

				protected override void AcceptVisitor<TVisitor>(TVisitor visitor)
				{
					visitor.Visit(this);

					foreach (var production in Productions)
					{
						foreach (var symbol in production.Symbols)
						{
							switch (symbol)
							{
								case FragmentSymbol fragmentSymbol:
									fragmentSymbol.Fragment.Visit(visitor);
									break;
								case TokenSymbol tokenSymbol:
									tokenSymbol.Token.Visit(visitor);
									break;
							}
						}
					}
				}

				public TriviaSyntax AddProduction(TToken token, Production production)
				{
					AddProductionCore(token, production);

					return this;
				}
			}
		}
	}
}