// <copyright file="HierarchyDataPlainListView.IList.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyDataPlainListView : IList<object>
	{
		int ICollection<object>.Count => HierarchyView.VisibleFlatCount;

		bool ICollection<object>.IsReadOnly => true;

		void ICollection<object>.Add(object item)
		{
			throw new NotSupportedException();
		}

		void ICollection<object>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<object>.Contains(object item)
		{
			return HierarchyView.FindDataIndex(item) != -1;
		}

		void ICollection<object>.CopyTo(object[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		bool ICollection<object>.Remove(object item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return HierarchyView.GetDataEnumerator();
		}

		object IList<object>.this[int index]
		{
			get => HierarchyView.GetData(index);
			set => throw new NotSupportedException();
		}

		int IList<object>.IndexOf(object item)
		{
			return HierarchyView.FindDataIndex(item);
		}

		void IList<object>.Insert(int index, object item)
		{
			throw new NotSupportedException();
		}

		void IList<object>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}
}