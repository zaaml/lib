// <copyright file="Parser.Automata.IParserEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private interface IParserEntry
			{
				#region Properties

				Grammar<TToken>.ParserEntry GrammarEntry { get; }

				ProductionArgument ProductionArgument { get; set; }

				#endregion
			}

			#endregion
		}

		#endregion
	}
}