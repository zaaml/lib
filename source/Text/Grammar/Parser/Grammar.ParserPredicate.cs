// <copyright file="Grammar.ParserPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class ParserPredicate : ParserEntry
		{
			#region Ctors

			public ParserPredicate(Parser<TToken>.PredicateEntry predicateEntry)
			{
				PredicateEntry = predicateEntry;
			}

			#endregion

			#region Properties

			public Parser<TToken>.PredicateEntry PredicateEntry { get; }

			#endregion
		}

		#endregion
	}
}