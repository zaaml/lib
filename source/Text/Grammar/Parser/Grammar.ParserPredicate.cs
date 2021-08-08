// <copyright file="Grammar.ParserPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class ParserPredicate : ParserEntry
		{
			public ParserPredicate(Parser<TToken>.PredicateEntry predicateEntry)
			{
				PredicateEntry = predicateEntry;
			}

			public Parser<TToken>.PredicateEntry PredicateEntry { get; }
		}
	}
}