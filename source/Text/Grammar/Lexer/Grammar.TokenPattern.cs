﻿// <copyright file="Grammar.TokenPattern.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed class TokenPattern
		{
			public TokenPattern(TokenEntry[] entries)
			{
				Entries = entries;
			}

			public TokenPattern(string stringPattern)
			{
				// TODO surrogate pairs handling

				Entries = new TokenEntry[stringPattern.Length];

				var index = 0;

				foreach (var ch in stringPattern)
					Entries[index++] = new CharEntry(ch);
			}

			public TokenEntry[] Entries { get; }

			public static implicit operator TokenPattern(string pattern)
			{
				return new TokenPattern(pattern);
			}

			public static implicit operator TokenPattern(Lexer<TToken>.PredicateEntry lexerPredicateEntry)
			{
				return new TokenPattern(new TokenEntry[] { new LexerPredicate(lexerPredicateEntry) });
			}

			public static implicit operator TokenPattern(Lexer<TToken>.ActionEntry lexerActionEntry)
			{
				return new TokenPattern(new TokenEntry[] { new LexerAction(lexerActionEntry) });
			}
		}
	}
}