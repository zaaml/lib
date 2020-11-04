// <copyright file="Grammar.TokenInterRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed class TokenInterRuleEntry : TokenInterEntry
		{
			#region Ctors

			public TokenInterRuleEntry(TokenRule tokenRule)
			{
				TokenRule = tokenRule;
			}

			#endregion

			#region Properties

			public TokenRule TokenRule { get; }

			#endregion

			#region Methods

			public override ParserEntry CreateParserEntry()
			{
				return new ParserTokenRuleEntry(TokenRule);
			}

			public static implicit operator TokenInterProduction(TokenInterRuleEntry entry)
			{
				return new TokenInterProduction(new TokenInterEntry[] {entry});
			}

			#endregion
		}

		#endregion
	}
}