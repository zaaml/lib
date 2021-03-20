// <copyright file="VirtualItemCollection.InnerTypes.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Collections;
using Zaaml.Core.Packed;

namespace Zaaml.UI.Controls.Core
{
	internal partial class VirtualItemCollection<T>
	{
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

			public bool IsInPool;

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

			public GeneratedItem Mount(T item, object source, ItemGenerator<T> generator, int generatorVersion, int collectionVersion)
			{
				Item = item;
				Source = source;
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

		protected enum OperatingMode
		{
			Real,
			Virtual
		}
		
		private readonly struct GeneratedIndexItemPair
		{
			public GeneratedIndexItemPair(int index, GeneratedItem item)
			{
				Item = item;
				Index = index;
			}

			public readonly GeneratedItem Item;

			public readonly int Index;

			public static readonly GeneratedIndexItemPair Empty = new GeneratedIndexItemPair(-1, null);

			public bool IsEmpty => Index == -1;

			public static implicit operator GeneratedIndexItemPair(SparseLinkedList<GeneratedItem>.IndexValuePair indexValuePair)
			{
				return new((int) indexValuePair.Index, indexValuePair.Value);
			}
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
					AddVoidRange(count - Count);
			}
		}

		private sealed class SparseLinkedListManager : SparseLinkedListManager<GeneratedItem>
		{
			public SparseLinkedListManager(VirtualItemCollection<T> virtualSource) : base(new SparseMemoryAllocator<GeneratedItem>(16))
			{
				VirtualSource = virtualSource;
			}

			private VirtualItemCollection<T> VirtualSource { get; }

			protected override void OnNodeReleasing(SparseLinkedListBase<GeneratedItem>.NodeBase node)
			{
				if (node is SparseLinkedListBase<GeneratedItem>.RealizedNode realizedNode)
					VirtualSource.ReleaseRealizedNode(realizedNode);

				base.OnNodeReleasing(node);
			}
		}
	}
}