// <copyright file="Parser.Automata.ParserPredicateKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private enum ParserPredicateKind
			{
				Generic,
				GenericValue,
				SubParser,
				SubLexer
			}

			#endregion
		}

		#endregion
	}
}