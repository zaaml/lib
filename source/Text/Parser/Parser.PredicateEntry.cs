// <copyright file="Parser.PredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
		public class PredicateEntry
		{
			public PredicateEntry(Func<ParserContext, bool> predicate)
			{
				Predicate = predicate;
			}

			public Func<ParserContext, bool> Predicate { get; }
		}
	}
}