// <copyright file="Grammar.RangeEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class RangeEntry : PrimitiveMatchEntry
		{
			#region Ctors

			public RangeEntry(char first, char last)
			{
				First = first;
				Last = last;
			}

			#endregion

			#region Properties

			public char First { get; }

			public char Last { get; }

			#endregion
		}

		#endregion
	}
}