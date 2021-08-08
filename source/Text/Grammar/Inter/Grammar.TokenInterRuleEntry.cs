// <copyright file="Grammar.TokenInterRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed class TokenInterRuleEntry : TokenInterEntry
		{
			public TokenInterRuleEntry(TokenRule tokenRule)
			{
				TokenRule = tokenRule;
			}

			public TokenRule TokenRule { get; }

			public override ParserEntry CreateParserEntry()
			{
				return new ParserTokenRuleEntry(TokenRule);
			}

			public static implicit operator TokenInterProduction(TokenInterRuleEntry entry)
			{
				return new TokenInterProduction(new TokenInterEntry[] { entry });
			}
		}
	}
}