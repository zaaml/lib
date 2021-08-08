// <copyright file="Parser.ActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
		public class ActionEntry
		{
			public ActionEntry(Action<ParserContext> action)
			{
				Action = action;
			}

			public Action<ParserContext> Action { get; }
		}
	}
}