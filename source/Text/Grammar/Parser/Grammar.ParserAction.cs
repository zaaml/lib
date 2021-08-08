// <copyright file="Grammar.ParserAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class ParserAction : ParserEntry
		{
			public ParserAction(Parser<TToken>.ActionEntry actionEntry)
			{
				ActionEntry = actionEntry;
			}

			public Parser<TToken>.ActionEntry ActionEntry { get; }
		}
	}
}