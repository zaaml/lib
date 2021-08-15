// <copyright file="Parser.Automata.StaticLexeme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private static readonly string[] StaticLexemes;

			//var grammar = Grammar.Get<TGrammar, TToken>();
			//var stringBuilder = new StringBuilder();

			//static bool BuildStaticLexeme(Grammar<TToken>.PatternCollection patternCollection, StringBuilder stringBuilder)
			//{
			//	if (patternCollection.Patterns.Count != 1)
			//		return false;

			//	var pattern = patternCollection.Patterns[0];

			//	foreach (var patternEntry in pattern.Entries)
			//	{
			//		if (patternEntry is Grammar<TToken>.CharEntry charEntry)
			//			stringBuilder.Append(charEntry.Char);
			//		else if (patternEntry is Grammar<TToken>.TokenFragment fragment)
			//		{
			//			if (BuildStaticLexeme(fragment.Pattern, stringBuilder) == false)
			//				return false;
			//		}
			//		else if (patternEntry is Grammar<TToken>.TokenFragmentEntry fragmentEntry)
			//		{
			//			if (BuildStaticLexeme(fragmentEntry.Fragment.Pattern, stringBuilder) == false)
			//				return false;
			//		}
			//		else
			//			return false;
			//	}

			//	return true;
			//}

			//foreach (var tokenRule in grammar.TokenRules)
			//{
			//	if (tokenRule.Pattern.Patterns.Count != 1)
			//		continue;

			//	stringBuilder.Clear();

			//	if (BuildStaticLexeme(tokenRule.Pattern, stringBuilder) == false)
			//		continue;

			//	if (stringBuilder.Length > 0)
			//		StaticLexemes[tokenRule.TokenCode] = stringBuilder.ToString();
			//}
		}
	}
}