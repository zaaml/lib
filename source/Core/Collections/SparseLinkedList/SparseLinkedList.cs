// <copyright file="SparseLinkedList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T> : SparseLinkedListBase<T>
	{
		public SparseLinkedList()
		{
		}

		internal SparseLinkedList(SparseLinkedListManager<T> manager) : base(0, manager)
		{
		}

		private bool Locked { get; set; }

		internal int Version { get; private set; }

		private void CopyToImpl(T[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex < Count)
				throw new InvalidOperationException("Insufficient array length");

			long index = arrayIndex;
			NodeBase current = HeadNode;

			while (current != null)
			{
				if (current is RealizedNode realizedNode)
				{
					//Array.Copy(realizedNode.ItemsPrivate, 0, array, index, realizedNode.Count);

					var sourceSpan = realizedNode.Span.Slice(0, (int) realizedNode.Size);
					var targetSpan = new Span<T>(array, (int) index, (int) realizedNode.Size);

					sourceSpan.CopyTo(targetSpan);
				}

				index += current.Size;

				current = current.Next;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Lock()
		{
			if (Locked)
				throw new InvalidOperationException();

			Locked = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Unlock()
		{
			if (Locked == false)
				throw new InvalidOperationException();

			Locked = false;
		}
	}
}