// <copyright file="Lexer.Automata.LexerProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerProduction : Production
			{
				public LexerProduction(Entry entry) : base(entry)
				{
				}

				public LexerProduction(IEnumerable<Entry> entries) : base(entries)
				{
				}
			}
		}
	}
}