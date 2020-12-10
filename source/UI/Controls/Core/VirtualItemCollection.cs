// <copyright file="VirtualItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Packed;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.Core
{
	internal class VirtualItemCollection<T> : IVirtualItemCollection where T : FrameworkElement
	{
		private const int NonGeneratedVersion = -1;

		private ItemGenerator<T> _generator;
		private uint _packedValue;
		private IEnumerable _source;

		protected VirtualItemCollection()
		{
			LinkedListManager = new SparseLinkedListManager(this);
			TempGeneratedItems = new GeneratedItemList(LinkedListManager);
			NextGeneratedItems = new GeneratedItemList(LinkedListManager);
			PrevGeneratedItems = new GeneratedItemList(LinkedListManager);
		}

		public IEnumerable<T> ActualItems => GeneratedItems;

		private Stack<GeneratedItem> GeneratedItemPool { get; } = new Stack<GeneratedItem>();

		private IEnumerable<T> GeneratedItems => PrevGeneratedItems.Where(g => g != null).Select(g => g.Item).Where(i => i != null);

		public ItemGenerator<T> Generator
		{
			get => _generator;
			set
			{
				if (ReferenceEquals(_generator, value))
					return;

				if (_generator != null)
				{
					ClearRecyclePool();
					DetachGenerator();
				}

				_generator = value;

				if (_generator != null)
				{
					AttachGenerator();
					IsRecycling = _generator.SupportsRecyclingInternal;
				}
			}
		}

		private int GeneratorVersion { get; set; }

		protected IndexedEnumerable IndexedSource { get; set; } = IndexedEnumerable.Empty;

		internal IndexedEnumerable IndexedSourceInternal => IndexedSource;

		private bool IsGenerating
		{
			get => PackedDefinition.IsGenerating.GetValue(_packedValue);
			set => PackedDefinition.IsGenerating.SetValue(ref _packedValue, value);
		}

		public bool IsRecycling
		{
			get => PackedDefinition.IsRecycling.GetValue(_packedValue);
			set
			{
				if (IsRecycling == value)
					return;

				PackedDefinition.IsRecycling.SetValue(ref _packedValue, value);

				if (value)
					return;

				ClearRecyclePool();
			}
		}

		private IVirtualItemsHost ItemHost { get; set; }

		private SparseLinkedListManager<GeneratedItem> LinkedListManager { get; }

		private Dictionary<T, GeneratedItem> LockedItemDictionary { get; } = new Dictionary<T, GeneratedItem>();

		private Dictionary<object, GeneratedItem> LockedItemSourceDictionary { get; } = new Dictionary<object, GeneratedItem>();

		private GeneratedItemList NextGeneratedItems { get; set; }

		private GeneratedItemList PrevGeneratedItems { get; set; }

		private SparseLinkedListBase<GeneratedItem>.RealizedNode RealizedNodePool { get; set; }

		private Stack<GeneratedItem> RecyclePool { get; } = new Stack<GeneratedItem>();

		public IEnumerable Source
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				{
					if (_source is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged -= ObservableSourceOnCollectionChanged;
				}

				_source = value;

				{
					if (_source is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged += ObservableSourceOnCollectionChanged;
				}

				IndexedSource = Source != null ? new IndexedEnumerable(Source) : IndexedEnumerable.Empty;

				Reset();
			}
		}

		private int SourceVersion { get; set; }

		private bool SuspendReleaseGeneratedItems
		{
			get => PackedDefinition.SuspendReleaseGeneratedItems.GetValue(_packedValue);
			set => PackedDefinition.SuspendReleaseGeneratedItems.SetValue(ref _packedValue, value);
		}

		private GeneratedItemList TempGeneratedItems { get; }

		private int Version { get; set; }

		private void AddLockedSource(GeneratedItem generatedItem)
		{
			LockedItemSourceDictionary.Add(generatedItem.Source, generatedItem);
		}

		private void AttachGenerator()
		{
			Generator.GeneratorChangedCore += OnGeneratorChanged;
		}

		private void AttachItem(int index, GeneratedItem generatedItem)
		{
			ItemHost?.OnItemAttaching(generatedItem.Item);

			generatedItem.Attach();

			if (ReferenceEquals(generatedItem.Item, generatedItem.Source) == false)
				generatedItem.Generator.AttachItemCore(generatedItem.Item, generatedItem.Source);

			OnGeneratedItemAttached(index, generatedItem.Item);

			ItemHost?.OnItemAttached(generatedItem.Item);
		}

		private bool CanReuse(GeneratedItem generatedItem, ItemGenerator<T> generator)
		{
			if (generatedItem.GeneratorVersion == NonGeneratedVersion)
				return true;

			return generatedItem.Item != null && (ReferenceEquals(generatedItem.Generator, generator) && generatedItem.GeneratorVersion == GeneratorVersion);
		}

		private void ClearRecyclePool()
		{
			while (RecyclePool.Count > 0)
				ReleaseItem(RecyclePool.Pop(), false);
		}

		private GeneratedItem CreateGeneratedItem()
		{
			return GeneratedItemPool.Count > 0 ? GeneratedItemPool.Pop() : new GeneratedItem(this);
		}

		private void DetachGenerator()
		{
			Generator.GeneratorChangedCore -= OnGeneratorChanged;
		}

		private void DetachItem(int index, GeneratedItem generatedItem)
		{
			ItemHost?.OnItemDetaching(generatedItem.Item);

			generatedItem.Detach();

			OnGeneratedItemDetached(index, generatedItem.Item);

			if (ReferenceEquals(generatedItem.Item, generatedItem.Source) == false)
				generatedItem.Generator.DetachItemCore(generatedItem.Item, generatedItem.Source);

			ItemHost?.OnItemDetached(generatedItem.Item);
		}

		private void EnsureCount()
		{
			var count = Count;

			NextGeneratedItems.EnsureCount(count);
			PrevGeneratedItems.EnsureCount(count);
		}

		public T EnsureItem(int index)
		{
			return GetItemFromIndex(index) ?? Realize(index);
		}

		private static IEnumerable<GeneratedItemIndexPair> EnumerateRealizedItems(GeneratedItemList generatedItems)
		{
			var currentNode = generatedItems.Head;

			if (currentNode.IsEmpty)
				yield break;

			while (currentNode.IsEmpty == false)
			{
				if (currentNode.IsRealized)
				{
					for (var i = 0; i < currentNode.Count; i++)
					{
						var generatedItem = currentNode[i];

						if (generatedItem != null)
							yield return new GeneratedItemIndexPair(generatedItem, currentNode.Index + i);
					}
				}

				currentNode = currentNode.Next;
			}
		}

		private IEnumerable<GeneratedItemIndexPair> EnumerateRealizedItems(GeneratedItemList generatedItems, int index, int count)
		{
			var currentNode = generatedItems.FindNode(index);

			if (currentNode.IsEmpty)
				yield break;

			var firstIndex = index - currentNode.Index;
			var currentIndex = 0;

			while (currentNode.IsEmpty == false && currentIndex < count)
			{
				if (currentNode.IsRealized)
				{
					for (var i = firstIndex; i < currentNode.Count && currentIndex < count; i++, currentIndex++)
					{
						var generatedItem = currentNode[i];

						if (generatedItem != null)
							yield return new GeneratedItemIndexPair(generatedItem, currentNode.Index + i);
					}
				}
				else
					currentIndex += currentNode.Count - firstIndex;

				firstIndex = 0;

				currentNode = currentNode.Next;
			}
		}

		private static GeneratedItemIndexPair FindGeneratedItem(T item, GeneratedItemList generatedItems)
		{
			var currentNode = generatedItems.Head;

			while (currentNode.IsEmpty == false)
			{
				if (currentNode.IsRealized)
				{
					for (var i = 0; i < currentNode.Count; i++)
					{
						var generatedItem = currentNode[i];

						if (generatedItem != null && ReferenceEquals(generatedItem.Item, item))
							return new GeneratedItemIndexPair(generatedItem, currentNode.Index + i);
					}
				}

				currentNode = currentNode.Next;
			}

			return GeneratedItemIndexPair.Empty;
		}

		private GeneratedItemIndexPair FindGeneratedItem(T item)
		{
			return FindGeneratedItem(item, PrevGeneratedItems);
		}

		private GeneratedItemIndexPair FindGeneratedItemEverywhere(T item)
		{
			var generatedItem = FindGeneratedItem(item, PrevGeneratedItems);

			if (generatedItem.IsEmpty == false)
				return generatedItem;

			generatedItem = FindGeneratedItem(item, NextGeneratedItems);

			if (generatedItem.IsEmpty == false)
				return generatedItem;

			return FindGeneratedItem(item, TempGeneratedItems);
		}

		private GeneratedItem Generate(ItemGenerator<T> generator, int index, object itemSource, out GeneratedItemSource generatedItemSource, out bool attach)
		{
			generatedItemSource = GeneratedItemSource.New;

			if (itemSource is T item)
			{
				var generate = CreateGeneratedItem().Mount(item, itemSource, generator, NonGeneratedVersion, Version);

				attach = true;

				return generate;
			}

			var lockedItem = GetLockedFromSource(itemSource);

			if (lockedItem != null)
			{
				if (CanReuse(lockedItem, generator))
				{
					Debug.Assert(ReferenceEquals(itemSource, lockedItem.Source));
					Debug.Assert(lockedItem.IsAttached);

					lockedItem.Mount(lockedItem.Item, lockedItem.Source, generator, GeneratorVersion, Version);

					generatedItemSource = GeneratedItemSource.Locked;
					attach = false;

					return lockedItem;
				}

				RemoveLockedSource(lockedItem);
			}

			var poolItem = GetFromPool(generator);

			if (poolItem != null)
			{
				generatedItemSource = GeneratedItemSource.Pool;

				poolItem.Mount(poolItem.Item, itemSource, generator, GeneratorVersion, Version);

				attach = true;

				return poolItem;
			}

			item = generator.CreateItemCore(itemSource);

			var generatedItem = CreateGeneratedItem().Mount(item, itemSource, generator, GeneratorVersion, Version);

			attach = true;

			return generatedItem;
		}

		protected virtual T GetCurrent(int index)
		{
			if (index >= 0 && index < Count && Count > 0)
			{
				var nextItems = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;

				return nextItems[index].Item;
			}

			return null;
		}

		private GeneratedItem GetFromPool(ItemGenerator<T> generator)
		{
			while (RecyclePool.Count > 0)
			{
				var recycleGeneratedItem = RecyclePool.Pop();

				if (IsLocked(recycleGeneratedItem))
					continue;

				if (CanReuse(recycleGeneratedItem, generator))
					return recycleGeneratedItem;
			}

			return null;
		}

		public virtual int GetIndexFromItem(T item)
		{
			return FindGeneratedItem(item).Index;
		}

		public int GetIndexFromItemSource(object itemSource)
		{
			return IndexedSource.IndexOf(itemSource);
		}

		public T GetItemFromIndex(int index)
		{
			return index >= PrevGeneratedItems.Count ? null : PrevGeneratedItems[index]?.Item;
		}

		public object GetItemSourceFromIndex(int index)
		{
			return IndexedSource[index];
		}

		private GeneratedItem GetLockedFromSource(object source)
		{
			return source != null && LockedItemSourceDictionary.TryGetValue(source, out var generatedItem) ? generatedItem : null;
		}

		private void InsertRange(int index, ICollection items)
		{
			var itemsCount = items.Count;

			PrevGeneratedItems.InsertCleanRange(index, itemsCount);
			NextGeneratedItems.InsertCleanRange(index, itemsCount);
		}

		private static bool IsLocked(GeneratedItem generatedItem)
		{
			return generatedItem.IsLocked;
		}

		public void LockItem(T item)
		{
			if (LockedItemDictionary.TryGetValue(item, out var lockedItem) == false)
			{
				var generatedItem = FindGeneratedItemEverywhere(item).Item;

				if (generatedItem == null)
					return;

				generatedItem.Lock();
				LockedItemDictionary.Add(item, generatedItem);
				AddLockedSource(generatedItem);
			}
			else
			{
				lockedItem.Lock();
			}
		}

		private void ObservableSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SourceVersion++;

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:

					InsertRange(e.NewStartingIndex, e.NewItems);

					break;

				case NotifyCollectionChangedAction.Remove:

					RemoveRange(e.OldStartingIndex, e.OldItems);

					break;

				case NotifyCollectionChangedAction.Replace:

					RemoveRange(e.OldStartingIndex, e.OldItems);
					InsertRange(e.NewStartingIndex, e.NewItems);

					break;

#if !SILVERLIGHT
				case NotifyCollectionChangedAction.Move:

					RemoveRange(e.OldStartingIndex, e.OldItems);
					InsertRange(e.NewStartingIndex, e.NewItems);

					break;
#endif
				case NotifyCollectionChangedAction.Reset:

					Reset();

					break;

				default:

					throw new ArgumentOutOfRangeException();
			}

			EnsureCount();

			ObservableSourceOnCollectionChanged(e);
		}

		protected virtual void ObservableSourceOnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
		}

		protected virtual void OnGeneratedItemAttached(int index, T item)
		{
		}

		protected virtual void OnGeneratedItemDetached(int index, T item)
		{
		}

		private void OnGeneratorChanged(object sender, EventArgs e)
		{
			GeneratorVersion++;

			ClearRecyclePool();
		}

		private void PushIntoPool(GeneratedItem item)
		{
			RecyclePool.Push(item);
		}

		private void PushIntoTempItems(GeneratedItem item)
		{
			Debug.Assert(item != null);
			Debug.Assert(item.IsInTemp == false);

			item.IsInTemp = true;

			TempGeneratedItems.Add(item);
		}

		protected virtual T Realize(int index)
		{
			var generator = Generator;

			if (generator == null)
				return null;

			var isGenerating = IsGenerating;
			var nextItems = isGenerating ? NextGeneratedItems : PrevGeneratedItems;
			var prevItems = isGenerating ? PrevGeneratedItems : NextGeneratedItems;

			var generatedItem = nextItems[index];

			if (generatedItem?.Item != null)
			{
				Debug.Assert(generatedItem.CollectionVersion == Version);

				return generatedItem.Item;
			}

			generatedItem = prevItems[index];

			if (isGenerating)
				prevItems[index] = null;

			if (generatedItem != null && CanReuse(generatedItem, generator) == false)
			{
				ReleaseItem(generatedItem, false);
				generatedItem = null;
			}

			var itemSource = IndexedSource[index];

			if (generatedItem != null && IsLocked(generatedItem))
			{
				if (ReferenceEquals(generatedItem.Source, itemSource) == false)
				{
					ReleaseItem(generatedItem, true);
					RemoveLockedSource(generatedItem);

					generatedItem = null;
				}
			}

			if (generatedItem != null)
			{
				generatedItem.CollectionVersion = Version;
				nextItems[index] = generatedItem;

				return generatedItem.Item;
			}

			generatedItem = Generate(generator, index, itemSource, out var generatedItemSource, out var attach);

			RemoveFromTemp(generatedItem);

			generatedItem.CollectionVersion = Version;

			nextItems[index] = generatedItem;

			if (attach)
				AttachItem(index, generatedItem);

			return generatedItem.Item;
		}

		private void ReleaseItem(GeneratedItem generatedItem, bool pushIntoPool)
		{
			DetachItem(-1, generatedItem);

			if (ReferenceEquals(generatedItem.Item, generatedItem.Source))
				return;

			if (pushIntoPool && IsRecycling)
				PushIntoPool(generatedItem);
			else
			{
				generatedItem.Generator.DisposeItemCore(generatedItem.Item, generatedItem.Source);
				generatedItem.Release();
			}
		}

		private void ReleaseRealizedNode(SparseLinkedListBase<GeneratedItem>.RealizedNode realizedNode)
		{
			if (SuspendReleaseGeneratedItems)
				return;

			var items = realizedNode.Span;
			var itemsLength = items.Length;

			for (var index = 0; index < itemsLength; index++)
			{
				var generatedItem = items[index];

				if (generatedItem != null)
					PushIntoTempItems(generatedItem);
			}
		}

		private void RemoveFromTemp(GeneratedItem generatedItem)
		{
			if (generatedItem.IsInTemp == false)
				return;

			var tempGeneratedItemPair = FindGeneratedItem(generatedItem.Item, TempGeneratedItems);

			Debug.Assert(tempGeneratedItemPair.IsEmpty == false);

			TempGeneratedItems[tempGeneratedItemPair.Index] = null;
			generatedItem.IsInTemp = false;
		}

		private void RemoveLockedSource(GeneratedItem generatedItem)
		{
			if (LockedItemSourceDictionary.ContainsKey(generatedItem.Source))
				LockedItemSourceDictionary.Remove(generatedItem.Source);
			else
			{
				if (generatedItem.IsAttached)
					ReleaseItem(generatedItem, true);
			}
		}

		private void RemoveRange(int index, ICollection items)
		{
			var itemsCount = items.Count;

			RemoveRange(index, itemsCount, PrevGeneratedItems);
			RemoveRange(index, itemsCount, NextGeneratedItems);
		}

		private void RemoveRange(int index, int count, GeneratedItemList generatedItems)
		{
			foreach (var generatedItem in EnumerateRealizedItems(generatedItems, index, count))
				PushIntoTempItems(generatedItem.Item);

			SuspendReleaseGeneratedItems = true;

			generatedItems.RemoveRange(index, count);

			SuspendReleaseGeneratedItems = false;
		}

		private void Reset()
		{
			foreach (var generatedItem in EnumerateRealizedItems(PrevGeneratedItems))
				PushIntoTempItems(generatedItem.Item);

			SuspendReleaseGeneratedItems = true;

			PrevGeneratedItems.Clear();
			NextGeneratedItems.Clear();

			EnsureCount();

			SuspendReleaseGeneratedItems = false;
		}

		public void UnlockItem(T item)
		{
			if (LockedItemDictionary.TryGetValue(item, out var generatedItem) == false)
				LogService.LogWarning($"Item {item} is not locked.");
			else
			{
				if (generatedItem.Unlock() == false)
					return;

				LockedItemDictionary.Remove(item);
				RemoveLockedSource(generatedItem);
			}
		}

		public int Count => IndexedSource.Count;

		IVirtualItemsHost IVirtualItemCollection.ItemHost
		{
			get => ItemHost;
			set => ItemHost = value;
		}

		void IVirtualItemCollection.EnterGeneration()
		{
			if (IsGenerating)
				throw new InvalidOperationException();

			IsGenerating = true;

			foreach (var tempGeneratedItemPair in EnumerateRealizedItems(TempGeneratedItems))
			{
				var tempGeneratedItem = tempGeneratedItemPair.Item;

				tempGeneratedItem.IsInTemp = false;

				if (IsLocked(tempGeneratedItem) == false)
					ReleaseItem(tempGeneratedItem, true);
			}

			SuspendReleaseGeneratedItems = true;

			TempGeneratedItems.Clear();

			SuspendReleaseGeneratedItems = false;

			EnsureCount();
		}

		int IVirtualItemCollection.GetIndexFromItem(FrameworkElement frameworkElement)
		{
			return GetIndexFromItem((T) frameworkElement);
		}

		UIElement IVirtualItemCollection.Realize(int index)
		{
			return Realize(index);
		}

		UIElement IVirtualItemCollection.GetCurrent(int index)
		{
			return GetCurrent(index);
		}

		void IVirtualItemCollection.LeaveGeneration()
		{
			if (IsGenerating == false)
				throw new InvalidOperationException();

			var items = PrevGeneratedItems;

			PrevGeneratedItems = NextGeneratedItems;
			NextGeneratedItems = items;

			items.Clear();

			EnsureCount();

			Version++;
			IsGenerating = false;
		}

		private enum GeneratedItemSource
		{
			New,
			Pool,
			Locked
		}

		private sealed class GeneratedItem
		{
			public int CollectionVersion;

			public ItemGenerator<T> Generator;

			public int GeneratorVersion;

			public bool IsInTemp;

			public T Item;

			private int LockCount;

			public object Source;

			public GeneratedItem(VirtualItemCollection<T> itemCollection)
			{
				ItemCollection = itemCollection;
			}

			public bool IsAttached { get; private set; }

			public bool IsLocked => LockCount > 0;

			private VirtualItemCollection<T> ItemCollection { get; }

			private Stack<GeneratedItem> Pool => ItemCollection.GeneratedItemPool;

			public void Attach()
			{
				if (IsAttached)
					throw new InvalidOperationException();

				IsAttached = true;
			}

			public void Detach()
			{
				if (IsAttached == false)
					throw new InvalidOperationException();

				IsAttached = false;
			}

			public bool Lock()
			{
				LockCount++;

				return LockCount == 1;
			}

			public GeneratedItem Mount(T item, object itemSource, ItemGenerator<T> generator, int generatorVersion, int collectionVersion)
			{
				Item = item;
				Source = itemSource;
				Generator = generator;
				GeneratorVersion = generatorVersion;
				CollectionVersion = collectionVersion;

				return this;
			}

			public void Release()
			{
				Item = default;
				Source = default;
				Generator = default;
				GeneratorVersion = default;
				CollectionVersion = default;

				Pool.Push(this);
			}

			public override string ToString()
			{
				return Item == null ? "Empty" : Item.ToString();
			}

			public bool Unlock()
			{
				if (LockCount == 0)
					return false;

				LockCount--;

				return LockCount == 0;
			}
		}

		private readonly struct GeneratedItemIndexPair
		{
			public GeneratedItemIndexPair(GeneratedItem item, int index)
			{
				Item = item;
				Index = index;
			}

			public readonly GeneratedItem Item;

			public readonly int Index;

			public static readonly GeneratedItemIndexPair Empty = new GeneratedItemIndexPair(null, -1);

			public bool IsEmpty => Index == -1;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsGenerating;
			public static readonly PackedBoolItemDefinition IsRecycling;
			public static readonly PackedBoolItemDefinition SuspendReleaseGeneratedItems;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsGenerating = allocator.AllocateBoolItem();
				IsRecycling = allocator.AllocateBoolItem();
				SuspendReleaseGeneratedItems = allocator.AllocateBoolItem();
			}
		}

		private sealed class GeneratedItemList : SparseLinkedList<GeneratedItem>
		{
			public GeneratedItemList(SparseLinkedListManager<GeneratedItem> linkedListManager) : base(0, linkedListManager)
			{
			}

			public new GeneratedItem this[int index]
			{
				get => index >= Count ? null : base[index];
				set
				{
					EnsureCount(index + 1);

					base[index] = value;
				}
			}

			public void EnsureCount(int count)
			{
				if (count > Count)
					AddCleanRange(count - Count);
			}
		}

		private sealed class SparseLinkedListManager : SparseLinkedListManager<GeneratedItem>
		{
			public SparseLinkedListManager(VirtualItemCollection<T> virtualSource) : base(new SparseMemoryManager<GeneratedItem>(16))
			{
				VirtualSource = virtualSource;
			}

			private VirtualItemCollection<T> VirtualSource { get; }

			protected override void OnNodeReleasing(SparseLinkedListBase<GeneratedItem>.Node node)
			{
				if (node is SparseLinkedListBase<GeneratedItem>.RealizedNode realizedNode)
					VirtualSource.ReleaseRealizedNode(realizedNode);

				base.OnNodeReleasing(node);
			}
		}
	}

	internal class VirtualItemCollection<TControl, T> : VirtualItemCollection<T> where T : FrameworkElement
	{
	}
}