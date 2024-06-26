﻿// <copyright file="HierarchyDataPlainListView.INotifyCollectionChanged.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Zaaml.Core.Collections;
using Zaaml.Core.Collections.Specialized;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyFlatListView<TNode> : INotifyCollectionChanged where TNode : class
	{
		private event NotifyCollectionChangedEventHandler CollectionChanged;

		internal void RaiseChange(int index, int count)
		{
			var changeCollection = new RepeatCollection<object>(Math.Abs(count), null);

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgsEx(count > 0 ? NotifyCollectionChangedAction.Add : NotifyCollectionChangedAction.Remove, changeCollection, index)
			{
				OriginalChangedItems = changeCollection
			});
		}

		internal void RaiseReset()
		{
			CollectionChanged?.Invoke(this, Constants.NotifyCollectionChangedReset);
		}

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add => CollectionChanged += value;
			remove => CollectionChanged -= value;
		}
	}
}