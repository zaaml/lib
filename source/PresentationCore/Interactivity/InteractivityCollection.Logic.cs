// <copyright file="InteractivityCollection.Logic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

#pragma warning disable 108

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract partial class InteractivityCollection<T>
		where T : InteractivityObject
	{
		#region Fields

		private List<T> _innerCollection = new List<T>();

		#endregion

		#region Ctors

		internal InteractivityCollection(IInteractivityObject parent)
		{
			Parent = parent;
		}

		#endregion

		#region Properties

    protected int Version { get; private set; }

		protected bool IsLoaded { get; private set; }

		internal IInteractivityObject Parent { get; }

		#endregion

		#region  Methods

		private int AddImpl(T item)
		{
			InsertImpl(Count, item);

			return Count;
		}

	  public void Clear()
	  {
	    ClearImpl();
	  }

	  protected virtual void ClearCore()
	  {
	    var oldList = _innerCollection;

	    _innerCollection = null;

	    foreach (var item in oldList)
	      OnItemRemovedCore(item);

	    _innerCollection = oldList;

      ListImplementation.Clear();
	    Version++;
    }

		private void ClearImpl()
		{
		  ClearCore();
		}

		protected internal virtual void CopyMembers(InteractivityCollection<T> source)
		{
			var count = source.Count;

			if (_innerCollection.Capacity < count)
				_innerCollection.Capacity = count;

			for (var i = 0; i < count; i++)
				Add(source[i].DeepClone<T>());
		}

		internal abstract InteractivityCollection<T> CreateInstance(IInteractivityObject parent);

		protected virtual void BeginCopyMembers()
		{
		}

		protected virtual void EndCopyMembers()
		{
		}

		internal virtual InteractivityCollection<T> DeepClone(IInteractivityObject parent)
		{
			var clone = CreateInstance(parent);

			clone.BeginCopyMembers();

			clone.CopyMembers(this);

			clone.EndCopyMembers();

		  clone.Version = Version;

			return clone;
		}

		private void InsertImpl(int index, T item)
		{
			InsertItemCore(index, item);
		}

		private void InsertImplCore(int index, T item)
		{
			ListTImplementation.Insert(index, item);
		  Version++;

      OnItemAddedCore(item);
		}

		protected virtual void InsertItemCore(int index, T item)
		{
			InsertImplCore(index, item);
		}

		internal void Load(IInteractivityRoot root)
		{
			IsLoaded = true;

			LoadCore(root);
		}

		internal virtual void LoadCore(IInteractivityRoot root)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
				this[i].Load(root);
		}

		protected virtual void OnIsTargetLoadedChanged()
		{
		}

		protected virtual void OnItemAddedCore(T item)
		{
			item.Parent = Parent;
		}

		protected virtual void OnItemRemovedCore(T item)
		{
			item.Parent = null;
		}

		private void RemoveAtImpl(int index)
		{
			RemoveImpl(default(T), index);
		}

		private void RemoveCoreImpl(int index)
		{
			var item = ListTImplementation[index];

			ListTImplementation.RemoveAt(index);
		  Version++;

			OnItemRemovedCore(item);
		}

		private bool RemoveImpl(T item, int index)
		{
			if (index == -1)
				index = ListTImplementation.IndexOf(item);

			RemoveItem(index);

			return index != -1;
		}

		protected virtual void RemoveItem(int index)
		{
			RemoveCoreImpl(index);
		}

		protected virtual void SetItem(int index, T item)
		{
			SetItemCoreImpl(index, item);
		}

		private void SetItemImpl(int index, T item)
		{
			SetItem(index, item);
		}

		private void SetItemCoreImpl(int index, T item)
		{
			var currentItem = ListTImplementation[index];

      ListTImplementation[index] = item;
		  Version++;

      OnItemRemovedCore(currentItem);
			OnItemAddedCore(item);
		}

		internal void Unload(IInteractivityRoot root)
		{
			UnloadCore(root);
			IsLoaded = false;
		}

		internal virtual void UnloadCore(IInteractivityRoot root)
		{
			var count = Count;

			for(var i = 0; i < count; i++)
				this[i].Unload(root);
		}

		#endregion
	}
}