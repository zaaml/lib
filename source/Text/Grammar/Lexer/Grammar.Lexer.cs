// <copyright file="Grammar.Lexer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken> where TToken : unmanaged, Enum
	{
		#region Static Fields and Constants

		protected static readonly List<TokenFragment> CreatedTokenFragments = new List<TokenFragment>();
		protected static readonly List<TokenRule> CreatedTokenRules = new List<TokenRule>();
		protected static readonly List<ParserFragment> CreatedParserFragments = new List<ParserFragment>();
		protected static readonly List<ParserRule> CreatedParserRules = new List<ParserRule>();

		#endregion

		#region Properties

		protected static RangeEntry Any { get; } = new RangeEntry(char.MinValue, char.MaxValue);

		#endregion

		#region Methods

		protected static TokenFragment CreateTokenFragment([CallerMemberName] string name = null)
		{
			var tokenFragment = new TokenFragment
			{
				Name = name
			};

			CreatedTokenFragments.Add(tokenFragment);

			return tokenFragment;
		}

		protected static TokenRule CreateTokenRule(TToken token, [CallerMemberName] string name = null)
		{
			var tokenRule = new TokenRule(token)
			{
				Name = name
			};

			CreatedTokenRules.Add(tokenRule);

			return tokenRule;
		}

		protected static SetEntry Except(RangeEntry rangeEntry)
		{
			return Except(new SetEntry(new[] {rangeEntry}));
		}

		protected static SetEntry Except(CharEntry charEntry)
		{
			return Except(new SetEntry(new[] {charEntry}));
		}

		protected static SetEntry Except(SetEntry setEntry)
		{
			var listRanges = new List<RangeEntry>();
			var current = char.MinValue;

			foreach (var entry in setEntry.Matches.OrderBy(GetMinChar))
			{
				if (entry is RangeEntry range)
				{
					if (range.First > current)
					{
						var next = (char) (range.First - 1);

						if (current < next)
							listRanges.Add(new RangeEntry(current, next));
					}

					current = (char) (range.Last + 1);

					continue;
				}

				var charEntry = (CharEntry) entry;
				var prev = (char) (charEntry.Char - 1);

				if (current < prev)
					listRanges.Add(new RangeEntry(current, prev));

				current = (char) (charEntry.Char + 1);
			}

			if (current < char.MaxValue)
				listRanges.Add(new RangeEntry(current, char.MaxValue));

			return new SetEntry(listRanges);
		}

		private static char GetMinChar(PrimitiveMatchEntry entry)
		{
			if (entry is RangeEntry range)
				return range.First;

			return ((CharEntry) entry).Char;
		}

		protected static TokenFragment Literal(string str)
		{
			return new TokenFragment(str);
		}

		protected static CharEntry Match(char @char)
		{
			return new CharEntry(@char);
		}

		protected static RangeEntry MatchRange(char first, char last)
		{
			return new RangeEntry(first, last);
		}

		protected static SetEntry MatchSet(params PrimitiveMatchEntry[] matches)
		{
			return new SetEntry(matches);
		}

		protected static TokenRuleSet MatchSet(params TokenRule[] tokenRules)
		{
			return new TokenRuleSet(tokenRules);
		}

		#endregion
	}
}