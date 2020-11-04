// <copyright file="Grammar.ParserAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class ParserAction : ParserEntry
		{
			#region Ctors

			public ParserAction(Parser<TToken>.ActionEntry actionEntry)
			{
				ActionEntry = actionEntry;
			}

			#endregion

			#region Properties

			public Parser<TToken>.ActionEntry ActionEntry { get; }

			#endregion
		}

		#endregion
	}
}