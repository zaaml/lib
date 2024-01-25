// <copyright file="VirtualItemCollectionSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class VirtualItemCollectionSource<TControl, TItem> : ItemCollectionSourceBase<TControl, TItem>
		where TItem : FrameworkElement
		where TControl : System.Windows.Controls.Control
	{
		private IEnumerable _source;

		public VirtualItemCollectionSource(IVirtualItemsHost<TItem> itemsHost, ItemCollectionBase<TControl, TItem> itemCollection) : base(itemsHost, itemCollection)
		{
			VirtualCollection = new VirtualItemCollection(itemCollection);

			itemsHost.VirtualSource = VirtualCollection;
		}

		public override IEnumerable<TItem> Items => VirtualCollection.ActualItems;

		public override int Count => VirtualCollection.ActualCount;

		public override IEnumerable Source
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				_source = value;

				VirtualCollection.SourceCollection = value;

				OnInvalidated();
			}
		}

		private VirtualItemCollection<TItem> VirtualCollection { get; }

		public override void Dispose()
		{
			base.Dispose();

			((IVirtualItemsHost<TItem>) ItemsHost).VirtualSource = null;
		}

		public override TItem EnsureItem(int index)
		{
			return VirtualCollection.EnsureItem(index);
		}

		protected override int GetIndexFromItem(TItem item)
		{
			return VirtualCollection.GetIndexFromItem(item);
		}

		protected override int GetIndexFromSource(object source)
		{
			return VirtualCollection.GetIndexFromSource(source);
		}

		protected override TItem GetItemFromIndex(int index)
		{
			return VirtualCollection.GetItemFromIndex(index);
		}

		protected override object GetSourceFromIndex(int index)
		{
			return VirtualCollection.GetSourceFromIndex(index);
		}

		public override void LockItem(TItem item)
		{
			VirtualCollection.LockItem(item);
		}

		protected override void OnGeneratorChanged()
		{
			base.OnGeneratorChanged();

			VirtualCollection.Generator = Generator;
		}

		private void OnInvalidated()
		{
			(ItemsHost as Panel)?.InvalidateMeasure();
		}

		public override void UnlockItem(TItem item)
		{
			VirtualCollection.UnlockItem(item);
		}

		private class VirtualItemCollection : VirtualItemCollection<TItem>
		{
			public VirtualItemCollection(ItemCollectionBase<TControl, TItem> itemCollection)
			{
				ItemCollection = itemCollection;
			}

			private ItemCollectionBase<TControl, TItem> ItemCollection { get; }

			protected override void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				base.OnSourceCollectionChanged(e);

				ItemCollection.OnSourceCollectionChanged(e);
			}

			protected override void OnGeneratedItemAttached(int index, TItem item)
			{
				ItemCollection.AttachGeneratedItem(index, item);
			}

			protected override void OnGeneratedItemDetached(int index, TItem item)
			{
				ItemCollection.DetachGeneratedItem(index, item);
			}
		}
	}

	internal interface IVirtualItemCollection
	{
		int Count { get; }

		IVirtualItemsHost ItemHost { get; set; }

		void EnterGeneration();

		UIElement GetCurrent(int index);

		int GetIndexFromItem(UIElement frameworkElement);

		void LeaveGeneration();

		UIElement Realize(int index);

		event NotifyCollectionChangedEventHandler SourceCollectionChanged;
	}
}