// <copyright file="Parser.Automata.IParserPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private interface IParserPredicate
			{
				#region Properties

				ParserPredicateKind PredicateKind { get; }

				#endregion
			}

			#endregion
		}

		#endregion
	}
}