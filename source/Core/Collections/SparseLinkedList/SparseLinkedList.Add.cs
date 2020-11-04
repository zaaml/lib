// <copyright file="SparseLinkedList.Add.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		#region Methods

		private void AddCleanRangeImpl(int count)
		{
			InsertCleanRangeImpl(Count, count);
		}

		private void AddImpl(T item)
		{
			InsertImpl(Count, item);
		}

		private void AddRangeImpl(IEnumerable<T> collection)
		{
			InsertRangeImpl(Count, collection);
		}

		#endregion
	}
}