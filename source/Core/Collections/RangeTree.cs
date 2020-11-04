// <copyright file="RangeTree.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Core.Collections
{
	internal sealed class RangeTree<TItem, T> where T : IComparable<T>
	{
		#region Ctors

		internal RangeTree(IEnumerable<TItem> items, Func<TItem, Range<T>> rangeFactory)
		{
			_root = new Node(items.Select(i => RangeItem.Create(i, rangeFactory(i))).ToList());
		}

		internal RangeTree(IEnumerable<RangeItem<TItem, T>> items)
		{
			_root = new Node(items.ToList());
		}

		#endregion

		#region Properties

		private readonly Node _root;

		#endregion

		#region  Methods

		public List<TItem> Query(T value)
		{
			return _root.Query(value);
		}

		public TItem QueryFirstOrDefault(T value, TItem defaultItem = default(TItem))
		{
			var result = defaultItem;

			return _root.QueryFirst(value, ref result) ? result : defaultItem;
		}

		public void Query(T value, ICollection<TItem> result)
		{
			_root.Query(value, result);
		}

		#endregion

		#region  Nested Types

		private class Node
		{
			#region Ctors

			public Node(ICollection<RangeItem<TItem, T>> items)
			{
				var endPoints = new List<T>();

				foreach (var item in items)
				{
					var range = item.Range;

					endPoints.Add(range.Minimum);
					endPoints.Add(range.Maximum);
				}

				endPoints.Sort();

				Center = endPoints.Count > 0 ? endPoints[endPoints.Count / 2] : default(T);

				Items = new List<RangeItem<TItem, T>>();

				var left = new List<RangeItem<TItem, T>>();
				var right = new List<RangeItem<TItem, T>>();

				foreach (var item in items)
				{
					var range = item.Range;

					if (range.Maximum.CompareTo(Center) < 0)
						left.Add(item);
					else if (range.Minimum.CompareTo(Center) > 0)
						right.Add(item);
					else
						Items.Add(item);
				}

				if (Items.Count > 0)
					Items.Sort();
				else
					Items = null;

				if (left.Count > 0)
					Left = new Node(left);

				if (right.Count > 0)
					Right = new Node(right);
			}

			#endregion

			#region Properties

			private T Center { get; }

			private List<RangeItem<TItem, T>> Items { get; }

			private Node Left { get; }

			private Node Right { get; }

			#endregion

			#region  Methods

			public List<TItem> Query(T value)
			{
				var result = new List<TItem>();

				Query(value, result);

				return result;
			}

			public bool QueryFirst(T value, ref TItem result)
			{
				if (Items != null)
				{
					foreach (var item in Items)
					{
						var range = item.Range;

						if (range.Minimum.CompareTo(value) > 0)
							break;

						if (range.Contains(value))
						{
							result = item.Item;

							return true;
						}
					}
				}

				if (value.CompareTo(Center) < 0 && Left != null)
					return Left.QueryFirst(value, ref result);

				if (value.CompareTo(Center) > 0 && Right != null)
					return Right.QueryFirst(value, ref result);

				return false;
			}

			public void Query(T value, ICollection<TItem> result)
			{
				if (Items != null)
				{
					var itemsCount = Items.Count;

					for (var index = 0; index < itemsCount; index++)
					{
						var item = Items[index];
						var range = item.Range;

						if (range.Minimum.CompareTo(value) > 0)
							break;

						if (range.Contains(value))
							result.Add(item.Item);
					}
				}

				if (value.CompareTo(Center) < 0 && Left != null)
					Left.Query(value, result);
				else if (value.CompareTo(Center) > 0 && Right != null)
					Right.Query(value, result);
			}

			#endregion
		}

		#endregion
	}

	internal static class RangeTree
	{
		#region  Methods

		public static RangeTree<TItem, T> Build<TItem, T>(IEnumerable<TItem> items, Func<TItem, Range<T>> rangeFactory) where T : IComparable<T>
		{
			return new RangeTree<TItem, T>(items, rangeFactory);
		}

		#endregion
	}
}