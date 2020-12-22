// <copyright file="Selection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal static class Selection
	{
		public static Selection<T> Create<T>(int index, T item, object source, object value)
		{
			return Selection<T>.Create(index, item, source, value);
		}

		public static Selection<T> Empty<T>() => Selection<T>.Empty;
	}

	public readonly struct Selection<T>
	{
		internal static readonly Selection<T> Empty = new Selection<T>(-1, default, null, null);

		public Selection(int index, T item, object source, object value)
		{
			Index = index;
			Item = item;
			Source = source;
			Value = value;
		}

		public bool IsEmpty => Index == -1 && Source == null && Value == null && Equals(Item, default);

		internal Selection<T> WithIndex(int index)
		{
			return Create(index, Item, Source, Value);
		}

		public static Selection<T> Create(int index, T item, object source, object value)
		{
			return new Selection<T>(index, item, source, value);
		}

		internal Selection<T> WithItem(T item)
		{
			return Create(Index, item, Source, Value);
		}

		internal Selection<T> WithSource(object source)
		{
			return Create(Index, Item, source, Value);
		}

		internal Selection<T> WithValue(object value)
		{
			return Create(Index, Item, Source, value);
		}

		public bool Equals(Selection<T> other)
		{
			return Index == other.Index && Equals(Source, other.Source) && EqualityComparer<T>.Default.Equals(Item, other.Item) && Equals(Value, other.Value);
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

				hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Item);
				hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);

				return hashCode;
			}
		}

		public int Index { get; }
		
		public object Source { get; }
		
		public T Item { get; }
		
		public object Value { get; }
	}
}