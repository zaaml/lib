// <copyright file="Parser.PredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
		#region Nested Types

		public class PredicateEntry
		{
			#region Ctors

			public PredicateEntry(Func<ParserContext, bool> predicate)
			{
				Predicate = predicate;
			}

			#endregion

			#region Properties

			public Func<ParserContext, bool> Predicate { get; }

			#endregion
		}

		#endregion
	}
}