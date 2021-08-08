// <copyright file="Grammar.LexerAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class LexerAction : TokenEntry
		{
			public LexerAction(Lexer<TToken>.ActionEntry actionEntry)
			{
				ActionEntry = actionEntry;
			}

			public Lexer<TToken>.ActionEntry ActionEntry { get; }
		}
	}
}