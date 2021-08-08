// <copyright file="Grammar.TokenRuleSet.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class TokenRuleSet : ParserPrimitiveEntry
		{
			public TokenRuleSet(TokenRule[] tokenRules)
			{
				TokenRules = tokenRules;
			}

			public TokenRule[] TokenRules { get; }

			public TokenRuleSet Bind(string value)
			{
				Name = value;

				return this;
			}
		}
	}
}