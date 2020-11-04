// <copyright file="Grammar.TokenInterEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract class TokenInterEntry
		{
			#region Methods

			public abstract ParserEntry CreateParserEntry();

			public static TokenInterProductionBuilder operator +(TokenInterEntry op1, TokenInterEntry op2)
			{
				return new TokenInterProductionBuilder(op1) + new TokenInterProductionBuilder(op2);
			}

			public static TokenInterProductionBuilder operator +(TokenRule op1, TokenInterEntry op2)
			{
				return new TokenInterProductionBuilder(new TokenInterRuleEntry(op1)) + new TokenInterProductionBuilder(op2);
			}

			public static TokenInterProductionBuilder operator +(TokenInterEntry op1, TokenRule op2)
			{
				return new TokenInterProductionBuilder(op1) + new TokenInterProductionBuilder(new TokenInterRuleEntry(op2));
			}

			#endregion
		}

		#endregion
	}
}