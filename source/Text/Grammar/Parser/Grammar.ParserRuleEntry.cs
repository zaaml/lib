// <copyright file="Grammar.ParserRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class ParserRuleEntry : ParserPrimitiveEntry
		{
			#region Ctors

			public ParserRuleEntry(ParserRule rule)
			{
				Rule = rule;
				Name = rule.Name;
			}

			#endregion

			#region Properties

			public ParserRule Rule { get; }

			public bool IsReturn { get; internal set; }

			#endregion

			public ParserRuleEntry Return()
			{
				IsReturn = true;

				return this;
			}
		}

		#endregion
	}
}