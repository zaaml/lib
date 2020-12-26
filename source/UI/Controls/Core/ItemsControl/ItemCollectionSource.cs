// <copyright file="ItemCollectionSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class ItemCollectionSource<TControl, TItem> : ItemCollectionSourceBase<TControl, TItem>
		where TItem : FrameworkElement
		where TControl : System.Windows.Controls.Control
	{
		private IEnumerable _source;

		public ItemCollectionSource(IItemsHost<TItem> itemsHost, ItemCollectionBase<TControl, TItem> itemCollection) : base(itemsHost, itemCollection)
		{
		}

		public override IEnumerable<TItem> Items => GeneratedItems.Select(g => g.Item);

		public override int Count => GeneratedItems.Count;

		private List<GeneratedItem> GeneratedItems { get; set; } = new List<GeneratedItem>();

		public override IEnumerable Source
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				Clean();

				{
					if (_source is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged -= ObservableSourceOnCollectionChanged;
				}

				_source = value;

				{
					if (_source is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged += ObservableSourceOnCollectionChanged;
				}

				Init();
			}
		}

		private void Clean()
		{
			ClearItems();
		}

		private void ClearItems()
		{
			var processList = GeneratedItems;

			GeneratedItems = new List<GeneratedItem>();

			var index = 0;

			foreach (var element in processList)
			{
				ItemCollection.DetachGeneratedItem(index++, element.Item);

				DisposeItem(element);
			}
		}

		private static void DisposeItem(GeneratedItem element)
		{
			var generator = element.Generator;

			generator.DetachItemCore(element.Item, element.Source);
			generator.DisposeItemCore(element.Item, element.Source);
		}

		public override TItem EnsureItem(int index)
		{
			return GetItemFromIndex(index);
		}

		private GeneratedItem GenerateItem(object item)
		{
			var generator = Generator;
			var element = generator.CreateItemCore(item);

			generator.AttachItemCore(element, item);

			return new GeneratedItem
			{
				Source = item,
				Generator = generator,
				Item = element
			};
		}

		protected override int GetIndexFromItem(TItem item)
		{
			var index = 0;

			foreach (var generatedItem in GeneratedItems)
			{
				if (ReferenceEquals(item, generatedItem.Item))
					return index;

				index++;
			}

			return -1;
		}

		protected override int GetIndexFromSource(object source)
		{
			var index = 0;

			foreach (var generatedItem in GeneratedItems)
			{
				if (ReferenceEquals(source, generatedItem.Source))
					return index;

				index++;
			}

			return -1;
		}

		protected override TItem GetItemFromIndex(int index)
		{
			return GeneratedItems[index].Item;
		}

		protected override object GetSourceFromIndex(int index)
		{
			return GeneratedItems[index].Source;
		}

		private void Init()
		{
			ClearItems();
			InitItems();
		}

		private void InitItems()
		{
			if (Source == null || Generator == null)
				return;

			var index = 0;

			foreach (var item in Source)
			{
				var generatedItem = GenerateItem(item);

				GeneratedItems.Add(generatedItem);
				ItemCollection.AttachGeneratedItem(index++, generatedItem.Item);
			}
		}

		private void InsertRange(int index, ICollection items)
		{
			var processList = new List<GeneratedItem>(items.Count);

			foreach (var item in items)
				processList.Add(GenerateItem(item));

			GeneratedItems.InsertRange(index, processList);

			foreach (var generatedItem in processList)
				ItemCollection.AttachGeneratedItem(index++, generatedItem.Item);
		}

		public override void LockItem(TItem item)
		{
		}

		private void ObservableSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			try
			{
				if (Generator == null)
					return;

				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:

						InsertRange(args.NewStartingIndex, args.NewItems);

						break;

					case NotifyCollectionChangedAction.Remove:

						RemoveRange(args.OldStartingIndex, args.OldItems);

						break;

					case NotifyCollectionChangedAction.Replace:

						RemoveRange(args.OldStartingIndex, args.OldItems);
						InsertRange(args.NewStartingIndex, args.NewItems);

						break;

#if !SILVERLIGHT
					case NotifyCollectionChangedAction.Move:

						RemoveRange(args.OldStartingIndex, args.OldItems);
						InsertRange(args.NewStartingIndex, args.NewItems);

						break;
#endif
					case NotifyCollectionChangedAction.Reset:

						ClearItems();
						InitItems();

						break;

					default:

						throw new ArgumentOutOfRangeException();
				}
			}
			finally
			{
				ItemCollection.OnSourceCollectionChanged(args);
			}
		}

		protected override void OnGeneratorChanged()
		{
			base.OnGeneratorChanged();

			Init();
		}

		protected override void OnGeneratorVersionChanged()
		{
			base.OnGeneratorVersionChanged();

			Init();
		}

		private void RemoveRange(int index, ICollection items)
		{
			var processList = new List<GeneratedItem>(items.Count);

			for (var i = index; i < index + items.Count; i++)
				processList.Add(GeneratedItems[i]);

			GeneratedItems.RemoveRange(index, items.Count);

			foreach (var generatedItem in processList)
			{
				ItemCollection.DetachGeneratedItem(index++, generatedItem.Item);

				DisposeItem(generatedItem);
			}
		}

		public override void UnlockItem(TItem item)
		{
		}

		private struct GeneratedItem
		{
			public TItem Item;

			public object Source;

			public ItemGenerator<TItem> Generator;

			public override string ToString()
			{
				return Item == null ? "Empty" : Item.ToString();
			}
		}
	}
}