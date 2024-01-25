// <copyright file="Parser.ActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken>
	{
		public class ActionEntry
		{
			public ActionEntry(Action<Parser<TToken>> action)
			{
				Action = action;
			}

			public Action<Parser<TToken>> Action { get; }
		}
	}
}