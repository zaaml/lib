// <copyright file="Parser.Automata.ParserEntryData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private sealed class ParserEntryData
			{
				#region Fields

				public int FlatIndex = -1;

				#endregion

				#region Ctors

				public ParserEntryData(string name, IParserEntry parserEntry)
				{
					Name = name;
					ParserEntry = parserEntry;
				}

				#endregion

				#region Properties

				public string Name { get; set; }

				public IParserEntry ParserEntry { get; }

				public ParserProduction ParserProduction { get; set; }

				#endregion

				#region Methods

				public override string ToString()
				{
					return Name;
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}