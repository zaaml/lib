// <copyright file="SparseLinkedList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T> : SparseLinkedListBase<T>, ISparseList<T>
	{
		public SparseLinkedList()
		{
		}

		internal SparseLinkedList(SparseLinkedListManager<T> manager) : base(0, manager)
		{
		}

		private bool Locked { get; set; }

		private int Version { get; set; }

		private void CopyTo(T[] array, int arrayIndex)
		{
			Lock();

			CopyToImpl(array, arrayIndex);

			Unlock();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Conditional("DEBUG")]
		private void Lock()
		{
			if (Locked)
				throw new InvalidOperationException();

			Locked = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Conditional("DEBUG")]
		private void Unlock()
		{
			if (Locked == false)
				throw new InvalidOperationException();

			Locked = false;
		}
	}
}