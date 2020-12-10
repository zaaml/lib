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
		
		protected SparseLinkedList(int count, SparseLinkedListManager<T> manager) : base(count,  manager)
		{
		}

		public SparseLinkedListNode<T> Head => new SparseLinkedListNode<T>(HeadNode, this);

		private bool Locked { get; set; }

		public SparseLinkedListNode<T> Tail => new SparseLinkedListNode<T>(TailNode, this);

		private void CopyToImpl(T[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex < Count)
				throw new InvalidOperationException("Insufficient array length");

			var index = arrayIndex;
			var current = HeadNode;

			while (current != null)
			{
				if (current is RealizedNode realizedNode)
				{
					//Array.Copy(realizedNode.ItemsPrivate, 0, array, index, realizedNode.Count);

					var destinationSpan = new Span<T>(array, index, realizedNode.Count);
					
					realizedNode.Span.CopyTo(destinationSpan);
				}

				index += current.Count;

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