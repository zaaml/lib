// <copyright file="TreeFlatListView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Collections;

namespace Zaaml.Core.Trees
{
	internal abstract partial class TreeFlatListView<T> where T : class
	{
		public virtual int Count => CursorCore.Count;

		protected abstract TreeFlatCursor<T> CursorCore { get; }

		public T this[int index] => ElementAt(index);

		public virtual T ElementAt(int index)
		{
			return CursorCore.ElementAt(index);
		}

		public virtual ReadOnlyListEnumerator<T> GetEnumerator()
		{
			return new(this);
		}

		public virtual int IndexOf(T value)
		{
			return CursorCore.IndexOf(value);
		}
	}
}