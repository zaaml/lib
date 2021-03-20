// <copyright file="SparseLinkedListBase.Merge.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void MergeImpl(SparseLinkedListBase<T> sourceList)
		{
			if (ReferenceEquals(this, sourceList))
				throw new InvalidOperationException("Cannot split into self");

			if (ReferenceEquals(Manager, sourceList.Manager) == false)
				throw new InvalidOperationException("Manager of source list and target list must be the same");

			if (sourceList.LongCount == 0)
				return;

			try
			{
				EnterStructureChange();
				sourceList.EnterStructureChange();

				if (LongCount == 0)
					SwapImpl(sourceList);
				else
				{
					var first = new LinkedListStruct(this);
					var second = new LinkedListStruct(sourceList);

					first.Merge(ref second);

					first.Store(this);
					second.Store(sourceList);
				}
			}
			finally
			{
				LeaveStructureChange();
				sourceList.LeaveStructureChange();
			}
		}
	}
}