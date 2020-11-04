// <copyright file="Parser.Automata.ParserSetMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class ParserSetMatchEntry : SetMatchEntry, IParserEntry
			{
				#region Ctors

				public ParserSetMatchEntry(Grammar<TToken>.TokenRuleSet grammarEntry) : base(grammarEntry.TokenRules.Select(CreateLexerEntry))
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