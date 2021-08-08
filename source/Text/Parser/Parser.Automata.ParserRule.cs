// <copyright file="Parser.Automata.ParserRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserRule : Rule
			{
				public ParserRule(string name, bool inline) : base(name)
				{
					Inline = inline;
				}
			}
		}
	}
}