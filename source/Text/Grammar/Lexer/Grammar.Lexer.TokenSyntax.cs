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

				public bool Composite { get; private set; }

				public TokenSyntax AddProduction(TToken token, Production production)
				{
					AddProductionCore(token, production);

					return this;
				}

				protected override void SealCore()
				{
					var visitor = new TokenVisitor(this);

					Visit(visitor);

					Composite = visitor.Composite;

					base.SealCore();
				}

				private sealed class TokenVisitor : SyntaxVisitor
				{
					public TokenVisitor(TokenSyntax tokenSyntax)
					{
						TokenSyntax = tokenSyntax;
					}

					public bool Composite { get; private set; }
					
					public TokenSyntax TokenSyntax { get; }

					public override void Visit(Syntax syntax)
					{
						if (ReferenceEquals(syntax, TokenSyntax))
							return;

						if (syntax is TokenSyntax)
							Composite = true;
					}
				}
			}
		}
	}
}