// <copyright file="Grammar.ParserTokenRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class ParserTokenRuleEntry : ParserPrimitiveEntry
		{
			public ParserTokenRuleEntry(TokenRule tokenRule)
			{
				TokenRule = tokenRule;
			}

			public TokenRule TokenRule { get; }

			public ParserTokenRuleEntry Bind(string name)
			{
				Name = name;

				return this;
			}

			public static implicit operator ParserTokenRuleEntry(TokenRule tokenRule)
			{
				return new ParserTokenRuleEntry(tokenRule);
			}
		}
	}
}