// <copyright file="Grammar.SetEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class SetEntry : MatchEntry
		{
			#region Ctors

			public SetEntry(IEnumerable<PrimitiveMatchEntry> matches)
			{
				Matches = matches;
			}

			#endregion

			#region Properties

			public IEnumerable<PrimitiveMatchEntry> Matches { get; }

			#endregion
		}

		#endregion
	}
}