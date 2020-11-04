// <copyright file="Grammar.TokenRuleSet.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class TokenRuleSet : ParserPrimitiveEntry
		{
			#region Ctors

			public TokenRuleSet(TokenRule[] tokenRules)
			{
				TokenRules = tokenRules;
			}

			#endregion

			#region Properties

			public TokenRule[] TokenRules { get; }

			#endregion

			#region Methods

			public TokenRuleSet Bind(string value)
			{
				Name = value;

				return this;
			}

			#endregion
		}

		#endregion
	}
}