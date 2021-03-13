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