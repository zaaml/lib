// <copyright file="DependencyObjectCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.ObservableCollections;

namespace Zaaml.PresentationCore
{
	public class DependencyObjectCollectionRaw<T> : FreezableCollection<T> where T : DependencyObject
	{
		internal DependencyObjectCollectionRaw()
		{
		}
	}


	public class DependencyObjectCollectionBase<T> : DependencyObjectCollectionRaw<T> where T : DependencyObject
	{
		[UsedImplicitly] private readonly ObservableCollectionDispatcher<T> _dispatcher;

		public DependencyObjectCollectionBase()
		{
			_dispatcher = this.Dispatch(OnItemAdded, OnItemRemoved);
		}

		protected virtual void OnItemAdded(T obj)
		{
		}

		protected virtual void OnItemRemoved(T obj)
		{
		}
	}
}