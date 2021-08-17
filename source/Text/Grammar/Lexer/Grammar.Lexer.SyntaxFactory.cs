// <copyright file="Grammar.Lexer.SyntaxFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected static class SyntaxFactory
			{
				public static CharRangeSymbol Any { get; } = new(char.MinValue, char.MaxValue);

				public static CharSymbol Char(char c)
				{
					return new CharSymbol(c);
				}

#if NETCOREAPP
				public static CharRangeSymbol CharRange(Range charRange)
				{
					return new CharRangeSymbol((char)charRange.Start.Value, (char)charRange.End.Value);
				}
#endif

				public static CharRangeSymbol CharRange(char first, char last)
				{
					return new CharRangeSymbol(first, last);
				}

				public static CharSetSymbol CharSet(params PrimitiveMatchSymbol[] matches)
				{
					return new CharSetSymbol(matches);
				}

				public static CharSetSymbol Except(CharRangeSymbol charRangeSymbol)
				{
					return Except(new CharSetSymbol(new[] { charRangeSymbol }));
				}

				public static CharSetSymbol Except(CharSymbol charSymbol)
				{
					return Except(new CharSetSymbol(new[] { charSymbol }));
				}

				public static CharSetSymbol Except(CharSetSymbol charSetSymbol)
				{
					var listRanges = new List<CharRangeSymbol>();
					var current = char.MinValue;

					foreach (var entry in charSetSymbol.Matches.OrderBy(GetMinChar))
					{
						if (entry is CharRangeSymbol range)
						{
							if (range.First > current)
							{
								var next = (char)(range.First - 1);

								if (current < next)
									listRanges.Add(new CharRangeSymbol(current, next));
							}

							current = (char)(range.Last + 1);

							continue;
						}

						var charEntry = (CharSymbol)entry;
						var prev = (char)(charEntry.Char - 1);

						if (current < prev)
							listRanges.Add(new CharRangeSymbol(current, prev));

						current = (char)(charEntry.Char + 1);
					}

					if (current < char.MaxValue)
						listRanges.Add(new CharRangeSymbol(current, char.MaxValue));

					return new CharSetSymbol(listRanges);
				}

				public static FragmentSyntax Fragment(params Production[] lexerProductions)
				{
					var lexerFragment = new FragmentSyntax("Internal", true);

					if (lexerProductions == null)
						return lexerFragment;

					foreach (var lexerSyntaxProduction in lexerProductions)
						lexerFragment.AddProduction(lexerSyntaxProduction);

					return lexerFragment;
				}

				public static FragmentSyntax Fragment(Production lexerProduction)
				{
					return new FragmentSyntax("Internal", true).AddProduction(lexerProduction);
				}

				public static FragmentSyntax Fragment(Production lexerProduction1, Production lexerProduction2)
				{
					return new FragmentSyntax("Internal", true).AddProduction(lexerProduction1).AddProduction(lexerProduction2);
				}

				public static FragmentSyntax Fragment(Production lexerProduction1, Production lexerProduction2, Production lexerProduction3)
				{
					return new FragmentSyntax("Internal", true).AddProduction(lexerProduction1).AddProduction(lexerProduction2).AddProduction(lexerProduction3);
				}

				public static Production Production(params Symbol[] lexerEntries)
				{
					return new Production(lexerEntries);
				}

				public static FragmentSyntax String(string str)
				{
					// TODO surrogate pairs handling
					var entries = new Symbol[str.Length];

					var index = 0;

					foreach (var ch in str)
						entries[index++] = new CharSymbol(ch);

					return new FragmentSyntax("Internal", true).AddProduction(new Production(entries));
				}
			}
		}
	}
}