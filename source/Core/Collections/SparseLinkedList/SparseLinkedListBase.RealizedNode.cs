// <copyright file="SparseLinkedListBase.RealizedNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

			private MemorySpan<T> SparseMemorySpan { get; set; } = MemorySpan<T>.Empty;

			internal override T GetItem(ref NodeCursor cursor)
			{
#if DEBUG
				if (ContainsLocal(cursor.LocalIndex) == false)
					throw new IndexOutOfRangeException();
#endif

				return Span[cursor.LocalIndex];
			}

			internal override T GetLocalItem(int index)
			{
#if DEBUG
				if (ContainsLocal(index) == false)
					throw new IndexOutOfRangeException();
#endif

				return Span[index];
			}

			public void Mount(MemorySpan<T> sparseMemorySpan)
			{
				SparseMemorySpan = sparseMemorySpan;
			}

			public override void Release()
			{
				base.Release();

				SparseMemorySpan.Span.Clear();
				SparseMemorySpan.Dispose();
				SparseMemorySpan = MemorySpan<T>.Empty;
			}

			internal override void SetItem(ref NodeCursor cursor, T item)
			{
#if DEBUG
				if (ContainsLocal(cursor.LocalIndex) == false)
					throw new IndexOutOfRangeException();
#endif
				Span[cursor.LocalIndex] = item;
			}
		}
	}
}