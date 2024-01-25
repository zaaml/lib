// <copyright file="Parser.Automata.ParserPredicateKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private enum ParserPredicateKind
			{
				Generic,
				GenericValue,
				ExternalParser,
				ExternalLexer
			}
		}
	}
}