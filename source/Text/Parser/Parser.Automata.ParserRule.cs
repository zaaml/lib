// <copyright file="Parser.Automata.ParserRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserRule : Rule
			{
				public ParserRule(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax) : base(parserSyntax.Name)
				{
					Inline = parserSyntax is Grammar<TGrammar, TToken>.ParserGrammar.FragmentSyntax;
				}
			}
		}
	}
}