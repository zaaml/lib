// <copyright file="Selection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal static class Selection
	{
		public static Selection<T> Create<T>(int index, T item, object itemSource, object value)
		{
			return Selection<T>.Create(index, item, itemSource, value);
		}

		public static Selection<T> Empty<T>() => Selection<T>.Empty;
	}

	public readonly struct Selection<T>
	{
		internal static readonly Selection<T> Empty = new Selection<T>(-1, default, null, null);

		public Selection(int index, T item, object itemSource, object value)
		{
			Index = index;
			Item = item;
			ItemSource = itemSource;
			Value = value;
		}

		internal Selection<T> ChangeIndex(int index)
		{
			return Create(index, Item, ItemSource, Value);
		}

		public static Selection<T> Create(int index, T item, object itemSource, object value)
		{
			return new Selection<T>(index, item, itemSource, value);
		}

		internal Selection<T> ChangeItem(T item)
		{
			return Create(Index, item, ItemSource, Value);
		}

		internal Selection<T> ChangeItemSource(object itemSource)
		{
			return Create(Index, Item, itemSource, Value);
		}

		internal Selection<T> ChangeValue(object value)
		{
			return Create(Index, Item, ItemSource, value);
		}

		public bool Equals(Selection<T> other)
		{
			return Index == other.Index && Equals(ItemSource, other.ItemSource) && EqualityComparer<T>.Default.Equals(Item, other.Item) && Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is Selection<T> selection && Equals(selection);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Index;

				hashCode = (hashCode * 397) ^ (ItemSource != null ? ItemSource.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Item);
				hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);

				return hashCode;
			}
		}

		public readonly int Index;
		public readonly object ItemSource;
		public readonly T Item;
		public readonly object Value;
	}
}