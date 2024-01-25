// <copyright file="ItemHostCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class ItemHostCollection<TItem> : IEnumerable<TItem>
		where TItem : FrameworkElement
	{
		protected List<TItem> Items { get; } = new();

		internal void ClearInternal()
		{
			SyncCore(SyncAction.PreClear, SyncActionData.Empty);

			Items.Clear();

			SyncCore(SyncAction.PostClear, SyncActionData.Empty);
		}

		internal void InitInternal(ICollection<TItem> items)
		{
			SyncCore(SyncAction.PreInit, SyncActionData.Empty);

			Items.Clear();
			Items.AddRange(items);

			SyncCore(SyncAction.PostInit, SyncActionData.Empty);
		}

		internal void InsertInternal(int index, TItem item)
		{
			var syncActionData = new SyncActionData(index, item);

			SyncCore(SyncAction.PreInsert, syncActionData);

			Items.Insert(index, item);

			SyncCore(SyncAction.PostInsert, syncActionData);
		}

		internal void RemoveAtInternal(int index)
		{
			var syncActionData = new SyncActionData(index, Items[index]);

			SyncCore(SyncAction.PreRemove, syncActionData);

			Items.RemoveAt(index);

			SyncCore(SyncAction.PostRemove, syncActionData);
		}

		protected abstract void SyncCore(SyncAction syncAction, SyncActionData syncActionData);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		protected enum SyncAction
		{
			PreClear,
			PreInit,
			PreInsert,
			PreRemove,
			PostClear,
			PostInit,
			PostInsert,
			PostRemove
		}

		protected readonly struct SyncActionData
		{
			public SyncActionData(int index, TItem item)
			{
				Index = index;
				Item = item;
			}

			public static SyncActionData Empty => new(-1, null);

			public int Index { get; }

			public TItem Item { get; }
		}
	}
}