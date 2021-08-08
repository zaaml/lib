// <copyright file="Grammar.TokenRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class TokenRule : ParserPrimitiveEntry
		{
			public readonly TToken Token;

			internal int TokenCode;

			internal TokenRule(TToken token)
			{
				Token = token;
				GrammarType = GetGrammarType();
			}

			public Grammar<TToken> Grammar => Get<TToken>(GrammarType);

			private Type GrammarType { get; }

			public PatternCollection Pattern { get; set; }

			public bool Skip { get; set; }

			public ParserTokenRuleEntry Bind(string name)
			{
				return new ParserTokenRuleEntry(this)
				{
					Name = name
				};
			}

			internal string EnsureName()
			{
				return Name;
			}

			public static TokenInterProductionBuilder operator +(TokenRule op1, TokenRule op2)
			{
				return new TokenInterProductionBuilder() + op1 + op2;
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenRule op1, TokenRule op2)
			{
				return new TokenInterProductionCollectionBuilder() | op1 | op2;
			}
		}
	}
}