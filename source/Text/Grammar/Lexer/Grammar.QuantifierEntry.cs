// <copyright file="Grammar.QuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class QuantifierEntry : TokenEntry
		{
			public QuantifierEntry(TokenPrimitiveEntry primitiveEntry, QuantifierKind kind, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = kind;
				Mode = mode;
				Range = QuantifierHelper.GetRange(kind);
			}

			public QuantifierEntry(TokenPrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = QuantifierHelper.GetKind(range);
				Range = range;
				Mode = mode;
			}

			public QuantifierKind Kind { get; }

			public QuantifierMode Mode { get; }

			public TokenPrimitiveEntry PrimitiveEntry { get; }

			public Interval<int> Range { get; }
		}
	}
}