// <copyright file="HierarchyDataPlainListView.IList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyDataPlainListView : IList
	{
		#region  Methods

		public IEnumerator GetEnumerator()
		{
			return HierarchyView.GetDataEnumerator();
		}

		#endregion

		#region Interface Implementations

		#region IList

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value)
		{
			return HierarchyView.FindDataIndex(value) != -1;
		}

		int IList.IndexOf(object value)
		{
			return HierarchyView.FindDataIndex(value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		object IList.this[int index]
		{
			get => HierarchyView.GetData(index);
			set => throw new NotSupportedException();
		}

		bool IList.IsReadOnly => true;

		bool IList.IsFixedSize => false;

		#endregion

		#endregion
	}
}