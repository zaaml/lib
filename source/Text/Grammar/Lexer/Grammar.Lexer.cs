// <copyright file="Grammar.Lexer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		internal IEnumerable<LexerGrammar.FragmentSyntax> LexerSyntaxFragmentCollection => LexerGrammar.FragmentSyntaxCollection;

		internal IEnumerable<LexerGrammar.TokenSyntax> LexerSyntaxTokenCollection => LexerGrammar.TokenSyntaxCollection;

		internal IEnumerable<LexerGrammar.TriviaSyntax> LexerSyntaxTriviaCollection => LexerGrammar.TriviaSyntaxCollection;

		public partial class LexerGrammar
		{
			private static readonly Dictionary<string, FragmentSyntax> FragmentSyntaxDictionary = new();
			private static readonly Dictionary<string, TriviaSyntax> TriviaSyntaxDictionary = new();
			private static readonly Dictionary<string, TokenSyntax> TokenSyntaxDictionary = new();

			private static readonly Dictionary<int, Production> ProductionDictionary = new();

			internal static IEnumerable<FragmentSyntax> FragmentSyntaxCollection => FragmentSyntaxDictionary.Values;

			internal static IEnumerable<TokenSyntax> TokenSyntaxCollection => TokenSyntaxDictionary.Values;

			internal static IEnumerable<TriviaSyntax> TriviaSyntaxCollection => TriviaSyntaxDictionary.Values;

			private static int GetMinChar(PrimitiveMatchSymbol entry)
			{
				if (entry is CharRangeSymbol range)
					return range.First;

				return ((CharSymbol)entry).Char;
			}

			private protected static void RegisterFragmentSyntax(FragmentSyntax fragment)
			{
				FragmentSyntaxDictionary.Add(fragment.Name, fragment);
			}

			private protected static int RegisterProduction(Production production)
			{
				var index = ProductionDictionary.Count;

				ProductionDictionary.Add(index, production);

				return index;
			}

			private protected static void RegisterTokenSyntax(TokenSyntax token)
			{
				TokenSyntaxDictionary.Add(token.Name, token);
			}

			private protected static void RegisterTriviaSyntax(TriviaSyntax triviaSyntax)
			{
				TriviaSyntaxDictionary.Add(triviaSyntax.Name, triviaSyntax);
			}

			public void Seal()
			{
				foreach (var fragmentSyntax in FragmentSyntaxCollection)
					fragmentSyntax.Seal();

				foreach (var tokenSyntax in TokenSyntaxCollection)
					tokenSyntax.Seal();

				foreach (var triviaSyntax in TriviaSyntaxCollection)
					triviaSyntax.Seal();
			}
		}
	}
}