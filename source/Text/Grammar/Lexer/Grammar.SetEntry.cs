// <copyright file="Grammar.SetEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class SetEntry : MatchEntry
		{
			public SetEntry(IEnumerable<PrimitiveMatchEntry> matches)
			{
				Matches = matches;
			}

			public IEnumerable<PrimitiveMatchEntry> Matches { get; }
		}
	}
}