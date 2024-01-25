// <copyright file="Parser.PredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken>
	{
		public class PredicateEntry
		{
			public PredicateEntry(Func<Parser<TToken>, bool> predicate, string predicateName = null)
			{
				Predicate = predicate;
				PredicateName = predicateName;
			}

			public Func<Parser<TToken>, bool> Predicate { get; }

			public string PredicateName { get; }
		}
	}
}