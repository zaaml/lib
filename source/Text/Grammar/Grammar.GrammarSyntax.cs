// <copyright file="Grammar.GrammarSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		protected internal abstract class GrammarSyntax<TGrammarSyntax, TGrammarSyntaxProduction, TGrammarSymbol>
			where TGrammarSyntax : GrammarSyntax<TGrammarSyntax, TGrammarSyntaxProduction, TGrammarSymbol>
			where TGrammarSyntaxProduction : GrammarProduction<TGrammarSyntax, TGrammarSyntaxProduction, TGrammarSymbol>
			where TGrammarSymbol : GrammarSymbol<TGrammarSyntax, TGrammarSyntaxProduction, TGrammarSymbol>
		{
			protected GrammarSyntax(string name)
			{
				Name = name;
				Productions = new((TGrammarSyntax)this, ProductionList);
			}

			public Type GrammarType => typeof(TGrammar);

			public string Name { get; }

			private List<TGrammarSyntaxProduction> ProductionList { get; } = new();

			public GrammarProductionCollection<TGrammarSyntax, TGrammarSyntaxProduction, TGrammarSymbol> Productions { get; }

			public Type TokenType => typeof(TToken);

			private protected void AddProductionCore(TGrammarSyntaxProduction production)
			{
				ProductionList.Add(production);
			}
		}
	}
}