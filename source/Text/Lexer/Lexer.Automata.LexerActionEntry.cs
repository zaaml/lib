// <copyright file="Lexer.Automata.LexerActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerActionEntry : ActionEntry
			{
				public LexerActionEntry(Grammar<TGrammar, TToken>.LexerGrammar.ActionSymbol grammarEntry) : base(CreateActionDelegate(grammarEntry.Action))
				{
				}
			}
		}
	}
}