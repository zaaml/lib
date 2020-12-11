// <copyright file="SparseLinkedListBase - Copy (3).Realize.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		internal sealed class RealizedNode : NodeBase
		{
			public Span<T> Span => SparseMemorySpan.Span;

			private SparseMemorySpan<T> SparseMemorySpan { get; set; } = SparseMemorySpan<T>.Empty;

			internal override T GetLocalItem(int index)
			{
#if DEBUG
				if (ContainsLocal(index) == false)
					throw new IndexOutOfRangeException();
#endif

				return Span[index];
			}

			internal override T GetItem(ref NodeCursor cursor)
			{
#if DEBUG
				if (ContainsLocal(cursor.LocalIndex) == false)
					throw new IndexOutOfRangeException();
#endif

				return Span[cursor.LocalIndex];
			}

			internal override void SetItem(ref NodeCursor cursor, T item)
			{
#if DEBUG
				if (ContainsLocal(cursor.LocalIndex) == false)
					throw new IndexOutOfRangeException();
#endif
				Span[cursor.LocalIndex] = item;
			}

			public void Mount(SparseMemorySpan<T> sparseMemorySpan)
			{
				SparseMemorySpan = sparseMemorySpan;
			}

			public override void Release()
			{
				base.Release();

				SparseMemorySpan.Span.Clear();
				SparseMemorySpan.Release();
				SparseMemorySpan = SparseMemorySpan<T>.Empty;
			}
		}
	}
}