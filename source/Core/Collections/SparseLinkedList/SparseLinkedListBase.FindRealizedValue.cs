// <copyright file="SparseLinkedListBase.FindRealizedValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		// TODO Cover with unit tests
		internal IndexValuePair FindRealizedValue(T value)
		{
			var currentNode = Head;
			var currentNodeOffset = 0L;

			while (currentNode.IsEmpty == false)
			{
				if (currentNode.IsRealized)
				{
					for (var i = 0; i < currentNode.Count; i++)
					{
						var currentValue = currentNode[i];

						if (Equals(currentValue, value))
							return new IndexValuePair(currentNodeOffset + i, currentValue);
					}
				}

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}

			return IndexValuePair.Empty;
		}

		// TODO Cover with unit tests
		internal IndexValuePair FindRealizedValue(Func<T, bool> predicate)
		{
			var currentNode = Head;
			var currentNodeOffset = 0L;

			while (currentNode.IsEmpty == false)
			{
				if (currentNode.IsRealized)
				{
					for (var i = 0; i < currentNode.Count; i++)
					{
						var currentValue = currentNode[i];

						if (predicate(currentValue))
							return new IndexValuePair(currentNodeOffset + i, currentValue);
					}
				}

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}

			return IndexValuePair.Empty;
		}
	}
}