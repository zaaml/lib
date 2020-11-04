// <copyright file="HierarchyDataPlainListView.ChangeCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyDataPlainListView : INotifyCollectionChanged
	{
		#region Fields

		private event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		#region  Methods

		internal void RaiseChange(int index, int count)
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(count > 0 ? NotifyCollectionChangedAction.Add : NotifyCollectionChangedAction.Remove, new ChangeCollection(Math.Abs(count)), index));
		}

		internal void RaiseReset()
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		#endregion

		#region Interface Implementations

		#region INotifyCollectionChanged

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add => CollectionChanged += value;
			remove => CollectionChanged -= value;
		}

		#endregion

		#endregion

		#region  Nested Types

		private class ChangeCollection : IList
		{
			#region Ctors

			public ChangeCollection(int count)
			{
				Count = count;
			}

			#endregion

			#region Interface Implementations

			#region ICollection

			public int Count { get; }

			public object SyncRoot => this;

			public bool IsSynchronized => false;

			public void CopyTo(Array array, int index)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IEnumerable

			public IEnumerator GetEnumerator()
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IList

			public object this[int index]
			{
				get => throw new NotSupportedException();
				set => throw new NotSupportedException();
			}

			public bool IsReadOnly => true;

			public bool IsFixedSize => true;

			public int Add(object value)
			{
				throw new NotSupportedException();
			}

			public bool Contains(object value)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public int IndexOf(object value)
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			public void Remove(object value)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			#endregion

			#endregion
		}

		#endregion
	}
}