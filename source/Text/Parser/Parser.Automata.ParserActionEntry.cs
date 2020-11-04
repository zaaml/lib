// <copyright file="Parser.Automata.ParserActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private sealed class ParserActionEntry : ActionEntry, IParserEntry
			{
				#region Ctors

				public ParserActionEntry(Grammar<TToken>.ParserAction grammarEntry) : base(CreateActionDelegate(grammarEntry.ActionEntry))
				{
					ParserEntryData = new ParserEntryData(EnsureName(grammarEntry), this);
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