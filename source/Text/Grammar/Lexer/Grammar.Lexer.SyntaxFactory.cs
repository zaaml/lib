// <copyright file="Grammar.Lexer.SyntaxFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core;
using Zaaml.Text.Unicode;


#if NETCOREAPP
using Range = System.Range;
#endif

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			private const int MinOperandValue = 0;
			private const int MaxOperandValue = 0x10FFFF;

			protected internal abstract class ExternalSymbol : PrimitiveSymbol
			{
				public abstract Type ExternalGrammarType { get; }

				public abstract Type ExternalTokenType { get; }
			}

			protected internal abstract class ExternalNodeSymbol : ExternalSymbol
			{
				public abstract Type ExternalNodeType { get; }
			}

			protected internal sealed class ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> : ExternalNodeSymbol
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				public ExternalNodeSymbol(Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax<TExternalNode> externalParserNode)
				{
					ExternalParserNode = externalParserNode;
					ArgumentName = externalParserNode.EnsureName();
				}

				public override Type ExternalNodeType => typeof(TExternalNode);

				public override Type ExternalGrammarType => ExternalParserNode.GrammarType;

				public Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax<TExternalNode> ExternalParserNode { get; }

				public override Type ExternalTokenType => ExternalParserNode.TokenType;
			}

			protected static class SyntaxFactory
			{
				public static CharRangeSymbol Any { get; } = new(MinOperandValue, MaxOperandValue);

				public static ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> External<TExternalGrammar, TExternalToken, TExternalNode>(Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax<TExternalNode> externalParserRule)
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum
					where TExternalNode : class
				{
					return new ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode>(externalParserRule);
				}

				public static CharSymbol Char(char c)
				{
					if (char.IsSurrogate(c))
						throw new InvalidOperationException();

					return new CharSymbol(c);
				}

#if NETCOREAPP
				public static CharRangeSymbol CharRange(Range charRange)
				{
					return new CharRangeSymbol(charRange.Start.Value, charRange.End.Value);
				}
#endif

				public static CharRangeSymbol CharRange(char first, char last)
				{
					if (char.IsSurrogate(first) || char.IsSurrogate(last))
						throw new InvalidOperationException();

					return new CharRangeSymbol(first, last);
				}

				public static CharSetSymbol CharSet(params PrimitiveMatchSymbol[] matches)
				{
					return new CharSetSymbol(matches);
				}

				public static CharSetSymbol UnicodeCategorySet(string unicodeCategorySetName)
				{
					return new CharSetSymbol(UnicodeCategoryRanges.GetByShortName(unicodeCategorySetName).Select(GetMatchSymbol));
				}

				private static PrimitiveMatchSymbol GetMatchSymbol(Range<int> range)
				{
					if (range.Minimum == range.Maximum)
						return new CharSymbol(range.Minimum);

					return new CharRangeSymbol(range.Minimum, range.Maximum);
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
					var current = MinOperandValue;

					foreach (var entry in charSetSymbol.Matches.OrderBy(GetMinChar))
					{
						if (entry is CharRangeSymbol range)
						{
							if (range.First > current)
							{
								var next = range.First - 1;

								if (current < next)
									listRanges.Add(new CharRangeSymbol(current, next));
							}

							current = range.Last + 1;

							continue;
						}

						var charEntry = (CharSymbol)entry;
						var prev = charEntry.Char - 1;

						if (current < prev)
							listRanges.Add(new CharRangeSymbol(current, prev));

						current = charEntry.Char + 1;
					}

					if (current < MaxOperandValue)
						listRanges.Add(new CharRangeSymbol(current, MaxOperandValue));

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

				public static ActionSymbol Action(ActionSyntax actionSyntax)
				{
					return new ActionSymbol(actionSyntax);
				}

				public static PredicateSymbol Predicate(PredicateSyntax predicateSyntax)
				{
					return new PredicateSymbol(predicateSyntax);
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