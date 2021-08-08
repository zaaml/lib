// <copyright file="Grammar.TokenFragment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class TokenFragment : TokenEntry
		{
			public TokenFragment()
			{
			}

			public TokenFragment(PatternCollection pattern)
			{
				Pattern = pattern;
			}

			public TokenFragment(TokenPattern pattern)
			{
				Pattern = pattern;
			}

			public PatternCollection Pattern { get; set; }

			internal int TokenCode { get; set; }

			public QuantifierEntry AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(this), QuantifierHelper.AtLeast(count), mode);
			}

			public QuantifierEntry Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(this), QuantifierHelper.Between(from, to), mode);
			}

			public QuantifierEntry Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(this), QuantifierHelper.Exact(count), mode);
			}

			public QuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(this), QuantifierKind.OneOrMore, mode);
			}

			//public static implicit operator TokenFragment(PrimitiveMatchEntry entry)
			//{
			//	return new TokenFragment(new TokenPattern(new TokenEntry[] { entry }));
			//}

			public static implicit operator TokenFragment(MatchEntry entry)
			{
				return new TokenFragment(new TokenPattern(new TokenEntry[] { entry }));
			}

			public QuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(this), QuantifierKind.ZeroOrMore, mode);
			}

			public QuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(this), QuantifierKind.ZeroOrOne, mode);
			}
		}
	}
}