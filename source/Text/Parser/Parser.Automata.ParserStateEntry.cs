// <copyright file="Parser.Automata.ParserStateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private sealed class ParserStateEntry : StateEntry, IParserEntry
			{
				#region Ctors

				public ParserStateEntry(string name, FiniteState state, bool fragment, bool tryReturn) : base(state)
				{
					Fragment = fragment;
					TryReturn = tryReturn;
					ParserEntryData = new ParserEntryData(name, this);
				}

				#endregion

				#region Properties

				public bool Fragment { get; }

				public bool TryReturn { get; }

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