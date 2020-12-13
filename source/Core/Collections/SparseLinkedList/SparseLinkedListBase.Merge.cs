// <copyright file="SparseLinkedListBase.Split.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void MergeImpl(SparseLinkedListBase<T> list)
		{
			if (ReferenceEquals(Manager, list.Manager) == false)
				throw new InvalidOperationException("Manager of source list and target list must be the same");

			if (list.LongCount == 0)
				return;

			try
			{
				if (ReferenceEquals(list.HeadNode, list.TailNode))
				{
					if (ReferenceEquals(HeadNode, TailNode))
						HeadNode.Size += list.HeadNode.Size;
					else
						TailNode.Size += list.HeadNode.Size;

					list.HeadNode.Size = 0;
				}
				else
				{
					if (TailNode.Size > 0)
					{
						if (list.HeadNode.Size > 0)
						{
							throw new NotImplementedException();
						}
						else
						{
							throw new NotImplementedException();
						}
					}
					else
					{
						if (list.HeadNode.Size > 0)
						{
							throw new NotImplementedException();
						}
						else
						{
							throw new NotImplementedException();
						}
					}
				}

				LongCount += list.LongCount;
				list.LongCount = 0;
			}
			finally
			{
				LeaveStructureChange();
				list.LeaveStructureChange();
			}
		}
	}
}