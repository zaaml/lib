// <copyright file="NotifyCollectionChangedEventArgsEx.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Specialized;

namespace Zaaml.Core.Collections
{
	internal class NotifyCollectionChangedEventArgsEx : NotifyCollectionChangedEventArgs
	{
		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action) : base(action)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] IList changedItems) : base(action, changedItems)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [NotNull] IList newItems, [NotNull] IList oldItems) : base(action, newItems, oldItems)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [NotNull] IList newItems, [NotNull] IList oldItems, int startingIndex) : base(action, newItems, oldItems, startingIndex)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] IList changedItems, int startingIndex) : base(action, changedItems, startingIndex)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] IList changedItems, int index, int oldIndex) : base(action, changedItems, index, oldIndex)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] object changedItem) : base(action, changedItem)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] object changedItem, int index) : base(action, changedItem, index)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] object changedItem, int index, int oldIndex) : base(action, changedItem, index, oldIndex)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] object newItem, [CanBeNull] object oldItem) : base(action, newItem, oldItem)
		{
		}

		public NotifyCollectionChangedEventArgsEx(NotifyCollectionChangedAction action, [CanBeNull] object newItem, [CanBeNull] object oldItem, int index) : base(action, newItem, oldItem, index)
		{
		}

		internal IList OriginalChangedItems { get; set; }
	}
}