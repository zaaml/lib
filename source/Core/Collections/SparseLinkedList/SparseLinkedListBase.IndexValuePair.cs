// <copyright file="SparseLinkedListBase.IndexValuePair.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		internal readonly struct IndexValuePair
		{
			public IndexValuePair(long index, T value)
			{
				Index = index;
				Value = value;
			}

			public long Index { get; }

			public T Value { get; }

			public static readonly IndexValuePair Empty = new(-1, default);

			public bool IsEmpty => Index == -1;
		}
	}
}