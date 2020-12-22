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
using Zaaml.Core.Collections;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.Core
{
	internal partial class VirtualItemCollection<T> : IVirtualItemCollection where T : FrameworkElement
	{
		private const int NonGeneratedVersion = -1;
		private readonly List<IItemCollectionObserver<T>> _observers = new List<IItemCollectionObserver<T>>();

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

		private GeneratedItemIndexPair CurrentAttachDetachItemPair { get; set; }

		private Stack<GeneratedItem> GeneratedItemPool { get; } = new Stack<GeneratedItem>();

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

		private Dictionary<T, GeneratedItem> LockedItemDictionary { get; } = new Dictionary<T, GeneratedItem>();

		private Dictionary<object, GeneratedItem> LockedSourceDictionary { get; } = new Dictionary<object, GeneratedItem>();

		private GeneratedItemList NextGeneratedItems { get; set; }

		private GeneratedItemList PrevGeneratedItems { get; set; }

		private SparseLinkedListBase<GeneratedItem>.RealizedNode RealizedNodePool { get; set; }

		private Stack<GeneratedItem> RecyclePool { get; } = new Stack<GeneratedItem>();

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
			LockedSourceDictionary.Add(generatedItem.Source, generatedItem);
		}

		private void AttachGenerator()
		{
			Generator.GeneratorChangedCore += OnGeneratorChanged;
		}

		private void AttachItem(int index, GeneratedItem generatedItem)
		{
			try
			{
				CurrentAttachDetachItemPair = new GeneratedItemIndexPair(generatedItem, index);

				ItemHost?.OnItemAttaching(generatedItem.Item);

				generatedItem.Attach();

				if (ReferenceEquals(generatedItem.Item, generatedItem.Source) == false)
					generatedItem.Generator.AttachItemCore(generatedItem.Item, generatedItem.Source);

				OnGeneratedItemAttached(index, generatedItem.Item);

				ItemHost?.OnItemAttached(generatedItem.Item);
			}
			finally
			{
				CurrentAttachDetachItemPair = GeneratedItemIndexPair.Empty;
			}
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
			try
			{
				CurrentAttachDetachItemPair = new GeneratedItemIndexPair(generatedItem, index);

				ItemHost?.OnItemDetaching(generatedItem.Item);

				generatedItem.Detach();

				OnGeneratedItemDetached(index, generatedItem.Item);

				if (ReferenceEquals(generatedItem.Item, generatedItem.Source) == false)
					generatedItem.Generator.DetachItemCore(generatedItem.Item, generatedItem.Source);

				ItemHost?.OnItemDetached(generatedItem.Item);
			}
			finally
			{
				CurrentAttachDetachItemPair = GeneratedItemIndexPair.Empty;
			}
		}

		private void EnsureCount()
		{
			var count = Count;

			NextGeneratedItems.EnsureCount(count);
			PrevGeneratedItems.EnsureCount(count);
		}

		private static IEnumerable<GeneratedItemIndexPair> EnumerateRealizedItems(GeneratedItemList generatedItems)
		{
			var currentNode = generatedItems.Head;
			var currentNodeOffset = 0L;

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
							yield return new GeneratedItemIndexPair(generatedItem, (int) (currentNodeOffset + i));
					}
				}

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}
		}

		private IEnumerable<GeneratedItemIndexPair> EnumerateRealizedItems(GeneratedItemList generatedItems, int index, int count)
		{
			var currentNode = generatedItems.FindNode(index, out var currentNodeOffset);

			if (currentNode.IsEmpty)
				yield break;

			var firstIndex = index - currentNodeOffset;
			var currentIndex = 0L;

			while (currentNode.IsEmpty == false && currentIndex < count)
			{
				if (currentNode.IsRealized)
				{
					for (var i = (int) firstIndex; i < currentNode.Count && currentIndex < count; i++, currentIndex++)
					{
						var generatedItem = currentNode[i];

						if (generatedItem != null)
							yield return new GeneratedItemIndexPair(generatedItem, (int) (currentNodeOffset + i));
					}
				}
				else
					currentIndex += currentNodeOffset - firstIndex;

				firstIndex = 0;

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}
		}

		private static GeneratedItemIndexPair FindGeneratedItem(T item, GeneratedItemList generatedItems)
		{
			var currentNode = generatedItems.Head;
			var currentNodeOffset = 0L;

			while (currentNode.IsEmpty == false)
			{
				if (currentNode.IsRealized)
				{
					for (var i = 0; i < currentNode.Count; i++)
					{
						var generatedItem = currentNode[i];

						if (generatedItem != null && ReferenceEquals(generatedItem.Item, item))
							return new GeneratedItemIndexPair(generatedItem, (int) (currentNodeOffset + i));
					}
				}

				currentNodeOffset += currentNode.Count;
				currentNode = currentNode.Next;
			}

			return GeneratedItemIndexPair.Empty;
		}

		private GeneratedItemIndexPair FindGeneratedItem(T item)
		{
			if (CurrentAttachDetachItemPair.Item != null && ReferenceEquals(item, CurrentAttachDetachItemPair.Item.Item))
				return CurrentAttachDetachItemPair;

			return FindGeneratedItem(item, IsGenerating ? NextGeneratedItems : PrevGeneratedItems);
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

		private GeneratedItem Generate(ItemGenerator<T> generator, int index, object source, out GeneratedItemSource generatedItemSource, out bool attach)
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
				var recycleGeneratedItem = RecyclePool.Pop();

				if (IsLocked(recycleGeneratedItem))
					continue;

				if (CanReuse(recycleGeneratedItem, generator))
					return recycleGeneratedItem;
			}

			return null;
		}

		private IEnumerable<T> GetGeneratedItems()
		{
			var generatedItems = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;

			return generatedItems.Where(g => g != null).Select(g => g.Item).Where(i => i != null);
		}

		private GeneratedItem GetLockedGeneratedItemFromSource(object source)
		{
			return source != null && LockedSourceDictionary.TryGetValue(source, out var generatedItem) ? generatedItem : null;
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

		private void ObservableSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ObservableSourceOnCollectionChangedPrivate(e);
		}

		private void ObservableSourceOnCollectionChangedPrivate(NotifyCollectionChangedEventArgs e)
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

		private T Realize(int index)
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

			generatedItem = Generate(generator, index, source, out var generatedItemSource, out var attach);

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
	}

	internal class VirtualItemCollection<TControl, T> : VirtualItemCollection<T> where T : FrameworkElement
	{
	}
}