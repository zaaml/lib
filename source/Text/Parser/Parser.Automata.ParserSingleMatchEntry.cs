// <copyright file="Parser.Automata.ParserSingleMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private sealed class ParserSingleMatchEntry : SingleMatchEntry, IParserEntry
			{
				#region Ctors

				public ParserSingleMatchEntry(string name, TToken operand) : base(operand)
				{
					ParserEntryData = new ParserEntryData(name, this);
				}

				#endregion

				#region Interface Implementations

				#region Parser<TGrammar,TToken>.ParserAutomata.IParserEntry

				public ParserEntryData ParserEntryData { get; }

				#endregion

				#endregion
			}

			#endregion
		}

		#endregion
	}
}