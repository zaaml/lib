// <copyright file="Parser.Automata.ParserQuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class ParserQuantifierEntry : QuantifierEntry, IParserEntry
			{
				#region Ctors

				public ParserQuantifierEntry(Grammar<TToken>.ParserQuantifierEntry grammarEntry, PrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode) : base(primitiveEntry, range, mode)
				{
					var name = EnsureName(grammarEntry);

					if (name == null)
						return;

					var primitiveEntryData = ((IParserEntry) PrimitiveEntry).ParserEntryData;

					primitiveEntryData.Name = name;
				}

				#endregion

				#region Interface Implementations

				#region Parser<TGrammar,TToken>.ParserAutomata.IParserEntry

				public ParserEntryData ParserEntryData => null;

				#endregion

				#endregion
			}

			#endregion
		}

		#endregion
	}
}