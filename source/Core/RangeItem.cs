// <copyright file="RangeItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core
{
	internal static class RangeItem
	{
		public static RangeItem<TItem, T> Create<TItem, T>(TItem item, Range<T> range) where T : IComparable<T>
		{
			return new RangeItem<TItem, T>(item, range);
		}
	}

	internal readonly struct RangeItem<TItem, T> where T : IComparable<T>
	{
		public RangeItem(TItem item, Range<T> range)
		{
			Item = item;
			Range = range;
		}

		public readonly TItem Item;

		public readonly Range<T> Range;

		public override string ToString()
		{
			return $"{Item}:{Range}";
		}

		public bool Equals(RangeItem<TItem, T> other)
		{
			return EqualityComparer<TItem>.Default.Equals(Item, other.Item) && Range.Equals(other.Range);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is RangeItem<TItem, T> item && Equals(item);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TItem>.Default.GetHashCode(Item) * 397) ^ Range.GetHashCode();
			}
		}
	}
}