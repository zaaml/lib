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

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.Core
{
	internal partial class VirtualItemCollection<T> : IVirtualItemCollection where T : FrameworkElement
	{
		private const int NonGeneratedVersion = -1;

		private readonly List<IItemCollectionObserver<T>> _observers = new();

		private ItemGenerator<T> _generator;
		private uint _packedValue;
		private IEnumerable _sourceCollection;

		protected VirtualItemCollection()
		{
			LinkedListManager = new SparseLinkedListManager(this);
			TempGeneratedItems = new GeneratedItemList(LinkedListManager);
			NextGeneratedItems = new GeneratedItemList(LinkedListManager);
			PrevGeneratedItems = new GeneratedItemList(LinkedListManager);
		}

		private GeneratedIndexItemPair CurrentAttachDetachIndexItemPair { get; set; }

		private AttachDetachOperation CurrentOperation { get; set; } = AttachDetachOperation.None;

		private Stack<GeneratedItem> GeneratedItemPool { get; } = new();

		private int GeneratorVersion { get; set; }

		protected IndexedEnumerable IndexedSource { get; set; } = IndexedEnumerable.Empty;

		internal IndexedEnumerable IndexedSourceInternal => IndexedSource;

		private bool IsGenerating
		{
			get => PackedDefinition.IsGenerating.GetValue(_packedValue);
			set => PackedDefinition.IsGenerating.SetValue(ref _packedValue, value);
		}

		private IVirtualItemsHost ItemHost { get; set; }

		private SparseLinkedListManager<GeneratedItem> LinkedListManager { get; }

		private Dictionary<T, GeneratedItem> LockedItemDictionary { get; } = new();

		private Dictionary<object, GeneratedItem> LockedSourceDictionary { get; } = new();

		private GeneratedItemList NextGeneratedItems { get; set; }

		private GeneratedItemList PrevGeneratedItems { get; set; }

		private Stack<GeneratedItem> RecyclePool { get; } = new();

		private int SourceVersion { get; set; }

		private bool SuspendReleaseGeneratedItems
		{
			get => PackedDefinition.SuspendReleaseGeneratedItems.GetValue(_packedValue);
			set => PackedDefinition.SuspendReleaseGeneratedItems.SetValue(ref _packedValue, value);
		}

		private GeneratedItemList TempGeneratedItems { get; }

		private int Version { get; set; }

		private int VirtualActualCount => IndexedSource.Count;

		private void AddLockedSource(GeneratedItem generatedItem)
		{
			LockedSourceDictionary.Add(generatedItem.Source, generatedItem);
		}

		private void AttachGenerator()
		{
			Generator.GeneratorChangedCore += OnGeneratorChanged;
		}

		private void AttachItem(int index, GeneratedItem generatedItem)
		{
			if (CurrentOperation != AttachDetachOperation.None)
				throw new InvalidOperationException();

			try
			{
				CurrentOperation = AttachDetachOperation.Attach;
				CurrentAttachDetachIndexItemPair = new GeneratedIndexItemPair(index, generatedItem);

				ItemHost?.OnItemAttaching(generatedItem.Item);

				generatedItem.Attach();

				if (ReferenceEquals(generatedItem.Item, generatedItem.Source) == false)
					generatedItem.Generator.AttachItemCore(generatedItem.Item, generatedItem.Source);

				OnGeneratedItemAttached(index, generatedItem.Item);

				ItemHost?.OnItemAttached(generatedItem.Item);
			}
			finally
			{
				CurrentOperation = AttachDetachOperation.None;
				CurrentAttachDetachIndexItemPair = GeneratedIndexItemPair.Empty;
			}
		}

		private bool CanReuse(GeneratedItem generatedItem, ItemGenerator<T> generator)
		{
			if (generatedItem.GeneratorVersion == NonGeneratedVersion)
				return true;

			return generatedItem.Item != null && ReferenceEquals(generatedItem.Generator, generator) && generatedItem.GeneratorVersion == GeneratorVersion;
		}

		private void ClearRecyclePool()
		{
			while (RecyclePool.Count > 0)
				ReleaseItem(PopFromPool(), false);
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
			if (CurrentOperation != AttachDetachOperation.None)
				throw new InvalidOperationException();

			try
			{
				CurrentOperation = AttachDetachOperation.Detach;
				CurrentAttachDetachIndexItemPair = new GeneratedIndexItemPair(index, generatedItem);

				ItemHost?.OnItemDetaching(generatedItem.Item);

				generatedItem.Detach();

				OnGeneratedItemDetached(index, generatedItem.Item);

				if (ReferenceEquals(generatedItem.Item, generatedItem.Source) == false)
					generatedItem.Generator.DetachItemCore(generatedItem.Item, generatedItem.Source);

				ItemHost?.OnItemDetached(generatedItem.Item);
			}
			finally
			{
				CurrentOperation = AttachDetachOperation.None;
				CurrentAttachDetachIndexItemPair = GeneratedIndexItemPair.Empty;
			}
		}

		private void EnsureCount()
		{
			var count = IndexedSource.Count;

			NextGeneratedItems.EnsureCount(count);
			PrevGeneratedItems.EnsureCount(count);
		}

		private static IEnumerable<GeneratedIndexItemPair> EnumerateRealizedItems(GeneratedItemList generatedItems)
		{
			foreach (var indexValuePair in generatedItems.EnumerateRealizedValues())
				yield return indexValuePair;
		}

		private static IEnumerable<GeneratedIndexItemPair> EnumerateRealizedItems(GeneratedItemList generatedItems, int index, int count)
		{
			foreach (var indexValuePair in generatedItems.EnumerateRealizedValues(index, count))
				yield return indexValuePair;
		}

		private static GeneratedIndexItemPair FindGeneratedItem(T item, GeneratedItemList generatedItems)
		{
			return generatedItems.FindRealizedValue(g => g != null && ReferenceEquals(g.Item, item));
		}
		
		private static GeneratedIndexItemPair FindGeneratedItemSource(object source, GeneratedItemList generatedItems)
		{
			return generatedItems.FindRealizedValue(g => g != null && ReferenceEquals(g.Source, source));
		}

		private GeneratedIndexItemPair FindGeneratedItem(T item)
		{
			if (CurrentAttachDetachIndexItemPair.Item != null && ReferenceEquals(item, CurrentAttachDetachIndexItemPair.Item.Item))
				return CurrentAttachDetachIndexItemPair;

			return FindGeneratedItem(item, IsGenerating ? NextGeneratedItems : PrevGeneratedItems);
		}

		private GeneratedIndexItemPair FindGeneratedItemEverywhere(T item)
		{
			var generatedItem = FindGeneratedItem(item, PrevGeneratedItems);

			if (generatedItem.IsEmpty == false)
				return generatedItem;

			generatedItem = FindGeneratedItem(item, NextGeneratedItems);

			if (generatedItem.IsEmpty == false)
				return generatedItem;

			return FindGeneratedItem(item, TempGeneratedItems);
		}

		private GeneratedItem Generate(ItemGenerator<T> generator, object source, out GeneratedItemSource generatedItemSource, out bool attach)
		{
			generatedItemSource = GeneratedItemSource.New;

			if (source is T item)
			{
				var generate = CreateGeneratedItem().Mount(item, source, generator, NonGeneratedVersion, Version);

				attach = true;

				return generate;
			}

			var lockedItem = GetLockedGeneratedItemFromSource(source);

			if (lockedItem != null)
			{
				if (CanReuse(lockedItem, generator))
				{
					Debug.Assert(ReferenceEquals(source, lockedItem.Source));
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

				poolItem.Mount(poolItem.Item, source, generator, GeneratorVersion, Version);

				attach = true;

				return poolItem;
			}

			item = generator.CreateItemCore(source);

			var generatedItem = CreateGeneratedItem().Mount(item, source, generator, GeneratorVersion, Version);

			attach = true;

			return generatedItem;
		}

		private GeneratedItem GetFromPool(ItemGenerator<T> generator)
		{
			while (RecyclePool.Count > 0)
			{
				var recycleGeneratedItem = PopFromPool();

				if (IsLocked(recycleGeneratedItem))
					continue;

				if (CanReuse(recycleGeneratedItem, generator))
					return recycleGeneratedItem;
			}

			return null;
		}

		private GeneratedItem GetLockedGeneratedItemFromSource(object source)
		{
			return source != null && LockedSourceDictionary.TryGetValue(source, out var generatedItem) ? generatedItem : null;
		}

		private void InsertRange(int index, ICollection items)
		{
			var itemsCount = items.Count;

			PrevGeneratedItems.InsertVoidRange(index, itemsCount);
			NextGeneratedItems.InsertVoidRange(index, itemsCount);
		}

		private static bool IsLocked(GeneratedItem generatedItem)
		{
			return generatedItem.IsLocked;
		}

		private void OnGeneratorChanged(object sender, EventArgs e)
		{
			GeneratorVersion++;

			ClearRecyclePool();
		}

		private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnSourceCollectionChangedPrivate(e);
		}

		private void OnSourceCollectionChangedPrivate(NotifyCollectionChangedEventArgs e)
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

			OnSourceCollectionChanged(e);
		}

		private GeneratedItem PopFromPool()
		{
			if (RecyclePool.Count > 0)
			{
				var generatedItem = RecyclePool.Pop();

				generatedItem.IsInPool = false;

				return generatedItem;
			}

			return null;
		}

		private void PushIntoPool(GeneratedItem generatedItem)
		{
			generatedItem.IsInPool = true;

			RecyclePool.Push(generatedItem);
		}

		private void PushIntoTempItems(GeneratedItem generatedItem)
		{
			Debug.Assert(generatedItem != null);
			Debug.Assert(generatedItem.IsInTemp == false);

			if (generatedItem.IsInTemp)
				return;

			generatedItem.IsInTemp = true;

			TempGeneratedItems.Add(generatedItem);
		}

		private void ReleaseItem(GeneratedItem generatedItem, bool pushIntoPool)
		{
			if (generatedItem.IsAttached == false)
			{
				if (Debugger.IsAttached) 
					Debugger.Break();

				generatedItem.Release();

				return;
			}

			DetachItem(-1, generatedItem);

			if (ReferenceEquals(generatedItem.Item, generatedItem.Source))
			{
				generatedItem.Release();

				return;
			}

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

		private void RemoveFromTemp(int index)
		{
			var generatedItem = TempGeneratedItems[index];

			TempGeneratedItems[index] = null;

			generatedItem.IsInTemp = false;
		}

		private void RemoveLockedSource(GeneratedItem generatedItem)
		{
			if (LockedSourceDictionary.ContainsKey(generatedItem.Source))
				LockedSourceDictionary.Remove(generatedItem.Source);
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
			if (Mode == OperatingMode.Real)
			{
				RealReset();

				return;
			}

			foreach (var generatedItem in EnumerateRealizedItems(PrevGeneratedItems))
				PushIntoTempItems(generatedItem.Item);

			SuspendReleaseGeneratedItems = true;

			PrevGeneratedItems.Clear();
			NextGeneratedItems.Clear();

			EnsureCount();

			SuspendReleaseGeneratedItems = false;
		}

		private T VirtualEnsureItem(int index)
		{
			return GetItemFromIndex(index) ?? VirtualRealize(index);
		}

		private void VirtualEnterGeneration()
		{
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

		private T VirtualGetCurrent(int index)
		{
			var count = IndexedSource.Count;

			if (count == 0 || index < 0 || index >= count)
				return null;

			var nextItems = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;

			return nextItems[index]?.Item;
		}

		private IEnumerable<T> VirtualGetGeneratedItems()
		{
			var generatedItems = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;

			return generatedItems.Where(g => g != null).Select(g => g.Item).Where(i => i != null);
		}

		private int VirtualGetIndexFromItem(T item)
		{
			return FindGeneratedItem(item).Index;
		}

		private int VirtualGetIndexFromSource(object source)
		{
			return IndexedSource.IndexOf(source);
		}

		private T VirtualGetItemFromIndex(int index)
		{
			var items = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;

			if (index < 0 || index >= items.Count)
				return null;

			var generatedItem = items[index];

			if (generatedItem != null)
				return generatedItem.Item;

			var source = GetSourceFromIndex(index);

			if (source != null)
			{
				if (LockedSourceDictionary.TryGetValue(source, out var lockedItem))
					return lockedItem.Item;
			}

			return null;
		}

		private object VirtualGetSourceFromIndex(int index)
		{
			return IndexedSource[index];
		}

		private void VirtualLeaveGeneration()
		{
			var items = PrevGeneratedItems;

			PrevGeneratedItems = NextGeneratedItems;
			NextGeneratedItems = items;

			items.Clear();

			EnsureCount();

			Version++;
			IsGenerating = false;
		}

		private void VirtualLockItem(T item)
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

		private T VirtualRealize(int index)
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

			var source = IndexedSource[index];

			if (generatedItem != null && IsLocked(generatedItem))
			{
				if (ReferenceEquals(generatedItem.Source, source) == false)
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

			generatedItem = Generate(generator, source, out _, out var attach);

			RemoveFromTemp(generatedItem);

			generatedItem.CollectionVersion = Version;

			nextItems[index] = generatedItem;

			if (attach)
			{
				if (isGenerating == false)
				{
					var tempItem = FindGeneratedItemSource(generatedItem.Source, TempGeneratedItems);

					if (tempItem.IsEmpty == false)
					{
						RemoveFromTemp(tempItem.Index);

						ReleaseItem(tempItem.Item, true);
					}
				}

				AttachItem(index, generatedItem);
			}

			return generatedItem.Item;
		}

		private void VirtualUnlockItem(T item)
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

		public event NotifyCollectionChangedEventHandler SourceCollectionChanged;

		private enum AttachDetachOperation
		{
			None,
			Attach,
			Detach
		}
	}
}