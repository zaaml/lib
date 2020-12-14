using System;

namespace Zaaml.Core.Collections
{
	internal readonly struct SparseMemorySpan<T>
	{
		public static readonly SparseMemorySpan<T> Empty = new SparseMemorySpan<T>(null, -1, -1);

		public SparseMemorySpan(SparseMemoryPage<T> page, int offset, int length)
		{
			Page = page;
			Offset = offset;
			Length = length;
		}

		public readonly SparseMemoryPage<T> Page;
		public readonly int Offset;
		public readonly int Length;

		public Span<T> Span => Page.GetSpan(Offset, Length);

		public bool IsEmpty => Page == null;

		public void Release()
		{
			Page.Deallocate(this);
		}
	}
}