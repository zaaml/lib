// <copyright file="Grammar.GrammarSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		protected internal abstract class GrammarSymbol<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarSyntax : GrammarSyntax<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarProduction : GrammarProduction<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarSymbol : GrammarSymbol<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
		{
			public string ArgumentName { get; set; }

			public Type GrammarType => typeof(TGrammar);

			public Type TokenType => typeof(TToken);
		}
	}
}