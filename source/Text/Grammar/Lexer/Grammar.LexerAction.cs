// <copyright file="Grammar.LexerAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class LexerAction : TokenEntry
		{
			#region Ctors

			public LexerAction(Lexer<TToken>.ActionEntry actionEntry)
			{
				ActionEntry = actionEntry;
			}

			#endregion

			#region Properties

			public Lexer<TToken>.ActionEntry ActionEntry { get; }

			#endregion
		}

		#endregion
	}
}