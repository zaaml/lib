// <copyright file="SparseLinkedListBase.Split.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void SplitAtImpl(long index, SparseLinkedListBase<T> targetList)
		{
			if (ReferenceEquals(this, targetList))
				throw new InvalidOperationException("Cannot split into self");

			if (ReferenceEquals(Manager, targetList.Manager) == false)
				throw new InvalidOperationException("Manager of source list and target list must be the same");

			if (index == LongCount)
				return;

			try
			{
				EnterStructureChange();
				targetList.EnterStructureChange();

				if (index == 0)
				{
					if (targetList.LongCount == 0)
						SwapImpl(targetList);
					else
					{
						var left = new LinkedListStruct(this);
						var right = new LinkedListStruct(targetList);

						left.Merge(ref right);

						left.Store(targetList);
						right.Store(this);
					}

					return;
				}

				ref var cursor = ref NavigateTo(index);

				if (targetList.Count == 0)
				{
					var left = new LinkedListStruct(this);
					var right = new LinkedListStruct(targetList);

					left.Split(ref cursor, ref right);

					left.Store(this);
					right.Store(targetList);
				}
				else
				{
					var left = new LinkedListStruct(this);
					var mid = new LinkedListStruct(Manager);
					var right = new LinkedListStruct(targetList);

					left.Split(ref cursor, ref mid);
					mid.Merge(ref right);

					left.Store(this);
					mid.Store(targetList);

					right.Release();
				}
			}
			finally
			{
				LeaveStructureChange();
				targetList.LeaveStructureChange();
			}
		}

		private protected void SwapImpl(SparseLinkedListBase<T> targetList)
		{
			try
			{
				EnterStructureChange();
				targetList.EnterStructureChange();

				var left = new LinkedListStruct(this);
				var right = new LinkedListStruct(targetList);

				left.Store(targetList);
				right.Store(this);
			}
			finally
			{
				LeaveStructureChange();
				targetList.LeaveStructureChange();
			}
		}
	}
}