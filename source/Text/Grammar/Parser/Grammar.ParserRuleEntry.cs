// <copyright file="Grammar.ParserRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class ParserRuleEntry : ParserPrimitiveEntry
		{
			public ParserRuleEntry(ParserRule rule)
			{
				Rule = rule;
				Name = rule.Name;
			}

			public bool IsReturn { get; internal set; }

			public ParserRule Rule { get; }

			public ParserRuleEntry Return()
			{
				IsReturn = true;

				return this;
			}
		}
	}
}