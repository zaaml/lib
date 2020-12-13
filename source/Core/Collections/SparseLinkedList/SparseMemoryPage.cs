using System;
using System.Buffers;

namespace Zaaml.Core.Collections
{
	internal sealed class SparseMemoryPage<T> : SharedObject<SparseMemoryPage<T>>
	{
		private readonly SparseMemoryManager<T> _memoryManager;
		private T[] _array;
		private int _offset;

		public SparseMemoryPage(SparseMemoryManager<T> memoryManager)
		{
			_memoryManager = memoryManager;
		}

		public SparseMemorySpan<T> Allocate(int length)
		{
			var span = new SparseMemorySpan<T>(this, _offset, length);

			_offset += length;

			AddReference();

			return span;
		}

		public bool CanAllocate(int length)
		{
			return _offset + length <= _array.Length;
		}

		public void Deallocate(SparseMemorySpan<T> sparseMemorySpan)
		{
			ReleaseReference();
		}

		public Span<T> GetSpan(int offset, int length)
		{
			return new Span<T>(_array, offset, length);
		}

		protected override void OnMount()
		{
			base.OnMount();

			_array = ArrayPool<T>.Shared.Rent(_memoryManager.PageSize);
		}

		protected override void OnReleased()
		{
			ArrayPool<T>.Shared.Return(_array, true);

			_array = null;
			_offset = 0;
			_memoryManager.ReleasePage(this);

			base.OnReleased();
		}
	}
}