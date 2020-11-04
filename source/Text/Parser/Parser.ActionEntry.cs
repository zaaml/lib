// <copyright file="Parser.ActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
		#region Nested Types

		public class ActionEntry
		{
			#region Ctors

			public ActionEntry(Action<ParserContext> action)
			{
				Action = action;
			}

			#endregion

			#region Properties

			public Action<ParserContext> Action { get; }

			#endregion
		}

		#endregion
	}
}