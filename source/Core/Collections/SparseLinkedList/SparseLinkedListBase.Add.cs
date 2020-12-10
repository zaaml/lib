// <copyright file="SparseLinkedListBase.Add.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void AddCleanRangeImpl(int count)
		{
			InsertCleanRangeImpl(Count, count);
		}

		private protected void AddImpl(T item)
		{
			InsertImpl(Count, item);
		}

		private protected void AddRangeImpl(IEnumerable<T> collection)
		{
			InsertRangeImpl(Count, collection);
		}
	}
}