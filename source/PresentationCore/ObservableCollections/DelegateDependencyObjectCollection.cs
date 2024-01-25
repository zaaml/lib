// <copyright file="DelegateDependencyObjectCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;

namespace Zaaml.PresentationCore.ObservableCollections
{
	public class DelegateDependencyObjectCollection<T> : DispatchedDependencyObjectCollection<T> where T : DependencyObject
	{
		private readonly Action<T> _onItemAdded;
		private readonly Action<T> _onItemRemoved;

		public DelegateDependencyObjectCollection(Action<T> onItemAdded, Action<T> onItemRemoved)
		{
			_onItemAdded = onItemAdded ?? DummyAction<T>.Instance;
			_onItemRemoved = onItemRemoved ?? DummyAction<T>.Instance;
		}

		protected override void OnItemAdded(T obj)
		{
			base.OnItemAdded(obj);

			_onItemAdded(obj);
		}

		protected override void OnItemRemoved(T item)
		{
			base.OnItemRemoved(item);

			_onItemRemoved(item);
		}
	}
}