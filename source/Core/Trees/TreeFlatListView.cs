// <copyright file="TreeFlatListView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal abstract partial class TreeFlatListView<T>
	{
		protected abstract int Count { get; }

		protected abstract IEnumerator<T> GetEnumerator();

		protected abstract T GetItem(int index);

		protected abstract int IndexOf(T value);
	}
}