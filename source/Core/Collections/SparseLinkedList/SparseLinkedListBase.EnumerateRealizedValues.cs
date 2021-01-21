// <copyright file="SparseLinkedListBase.EnumerateRealizedValues.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		// TODO Cover with unit tests
		internal IEnumerable<IndexValuePair> EnumerateRealizedValues()
		{
			var currentNode = Head;
			var currentNodeOffset = 0L;

			if (currentNode.IsEmpty)
				yield break;

			while (currentNode.IsEmpty == false)
			{
				if (currentNode.IsRealized)
				{
					for (var i = 0; i < currentNode.Count; i++)
					{
						var value = currentNode[i];

						if (value != null)
							yield return new IndexValuePair(currentNodeOffset + i, value);
					}
				}

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}
		}

		// TODO Cover with unit tests
		internal IEnumerable<IndexValuePair> EnumerateRealizedValues(long index, long count)
		{
			var currentNode = FindNode(index, out var currentNodeOffset);

			if (currentNode.IsEmpty)
				yield break;

			var firstIndex = index - currentNodeOffset;
			var currentIndex = 0L;

			while (currentNode.IsEmpty == false && currentIndex < count)
			{
				if (currentNode.IsRealized)
				{
					for (var i = (int) firstIndex; i < currentNode.Count && currentIndex < count; i++, currentIndex++)
					{
						var value = currentNode[i];

						if (value != null)
							yield return new IndexValuePair(currentNodeOffset + i, value);
					}
				}
				else
					currentIndex += currentNode.Count - firstIndex;

				firstIndex = 0;

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}
		}
	}
}