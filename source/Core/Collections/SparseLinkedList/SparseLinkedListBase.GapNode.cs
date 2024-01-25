// <copyright file="SparseLinkedListBase.VoidNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		internal sealed class VoidNode : NodeBase
		{
			internal override T GetItem(ref NodeCursor cursor)
			{
#if DEBUG
				if (ContainsLocal(cursor.LocalIndex) == false)
					throw new IndexOutOfRangeException();
#endif

				return default;
			}

			internal override T GetLocalItem(int index)
			{
#if DEBUG
				if (ContainsLocal(index) == false)
					throw new IndexOutOfRangeException();
#endif

				return default;
			}

			internal override void SetItem(ref NodeCursor cursor, T item)
			{
				throw new InvalidOperationException();
			}
		}
	}
}