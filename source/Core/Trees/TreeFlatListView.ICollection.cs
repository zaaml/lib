// <copyright file="TreeFlatListView.ICollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.Core.Trees
{
	internal partial class TreeFlatListView<T>
	{
		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		int ICollection.Count => Count;

		object ICollection.SyncRoot => this;

		bool ICollection.IsSynchronized => false;
	}
}