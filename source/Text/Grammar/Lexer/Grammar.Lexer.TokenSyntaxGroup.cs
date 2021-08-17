// <copyright file="Grammar.Lexer.TokenSyntaxGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class TokenSyntaxGroup
			{
				public TokenSyntaxGroup(TToken token)
				{
					Token = token;
					Productions = ProductionsList.AsReadOnly();
				}

				public IReadOnlyList<Production> Productions { get; }

				private List<Production> ProductionsList { get; } = new();

				public TToken Token { get; }

				internal void RegisterProduction(Production production)
				{
					ProductionsList.Add(production);
				}
			}
		}
	}
}