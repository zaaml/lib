// <copyright file="Grammar.TokenEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract class TokenEntry
		{
			#region Properties

#if DEBUG
			public string DebugName { get; set; }
#endif

			public string Name { get; internal set; }

			#endregion

			#region Methods

			public static implicit operator TokenEntry(char c)
			{
				return new CharEntry(c);
			}

			public static implicit operator TokenEntry(Lexer<TToken>.PredicateEntry lexerPredicateEntry)
			{
				return new LexerPredicate(lexerPredicateEntry);
			}

			public static implicit operator TokenEntry(Lexer<TToken>.ActionEntry lexerActionEntry)
			{
				return new LexerAction(lexerActionEntry);
			}

			#endregion
		}

		#endregion
	}
}