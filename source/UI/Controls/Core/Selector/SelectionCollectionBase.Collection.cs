// <copyright file="SelectionCollectionBase.Collection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	//public abstract partial class SelectionCollectionBase<TItem> : IList
	//{
	//	public void CopyTo(Array array, int index)
	//	{
	//		throw new NotSupportedException();
	//	}

	//	public int Count
	//	{
	//		get
	//		{
	//			var enumerator = GetEnumerator();

	//			var count = 0;

	//			while (enumerator.MoveNext()) 
	//				count++;

	//			enumerator.Dispose();

	//			return count;
	//		}
	//	}

	//	public bool IsSynchronized => false;

	//	public object SyncRoot => null;

	//	public int Add(object value)
	//	{
	//		throw new NotSupportedException();
	//	}

	//	public void Clear()
	//	{
	//		throw new NotSupportedException();
	//	}

	//	public bool Contains(object value)
	//	{
	//		return IndexOf(value) != -1;
	//	}

	//	public int IndexOf(object value)
	//	{
	//		if (value is Selection<TItem> selection)
	//			return this.FindIndex(s => s.Equals(selection));

	//		return -1;
	//	}

	//	public void Insert(int index, object value)
	//	{
	//		throw new NotSupportedException();
	//	}

	//	public void Remove(object value)
	//	{
	//		throw new NotSupportedException();
	//	}

	//	public void RemoveAt(int index)
	//	{
	//		throw new NotSupportedException();
	//	}

	//	public bool IsFixedSize => false;

	//	public bool IsReadOnly => true;

	//	public object this[int index]
	//	{
	//		get => this.ElementAt(index);
	//		set => throw new NotSupportedException();
	//	}
	//}
}