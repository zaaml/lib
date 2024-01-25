// <copyright file="Grammar.Lexer.TokenBaseSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal abstract class TokenBaseSyntax : Syntax
			{
				protected TokenBaseSyntax(string name) : base(name)
				{
					TokenGroups = TokenGroupList.AsReadOnly();
				}

				private Dictionary<TToken, TokenSyntaxGroup> TokenGroupDictionary { get; } = new();

				private List<TokenSyntaxGroup> TokenGroupList { get; } = new();

				public IReadOnlyList<TokenSyntaxGroup> TokenGroups { get; }

				protected void AddProductionCore(TToken token, Production production)
				{
					VerifyUnsealed();

					production.Bind(token);

					AddProductionCore(production);

					TokenGroupDictionary.GetValueOrCreate(token, CreateTokenGroup).RegisterProduction(production);
				}

				private TokenSyntaxGroup CreateTokenGroup(TToken token)
				{
					var tokenGroup = new TokenSyntaxGroup(token);

					TokenGroupList.Add(tokenGroup);

					return tokenGroup;
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
			}
		}
	}
}