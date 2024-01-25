// <copyright file="IntSpanItemMerger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Zaaml.Core.Collections.Pooled;
using Zaaml.Core.Pools;
using Zaaml.Core.Utils;

namespace Zaaml.Core
{
	internal ref struct IntSpanItemMerger<TItem, TValue>
	{
		private static long NonceGenerator;
		private static IPool<Implementation> SharedPool => StackPool.GetShared<Implementation>();
		private IPool<Implementation> _pool;
		private Implementation _implementation;
		private long _nonce;

		public IntSpanItemMerger(
			IEnumerable<TItem> spanItems,
			TItem boundingItem,
			Func<TItem, IntSpan> spanAccessor,
			Func<TItem, TValue> valueAccessor,
			Func<IntSpan, TValue, TItem> spanItemFactory,
			Func<IReadOnlyCollection<TValue>, TValue> valueMerger,
			bool skipZeroSpans)
		{
			_nonce = ++NonceGenerator;
			_pool = SharedPool;
			_implementation = _pool.Rent();
			_implementation.Mount(spanItems, boundingItem, spanAccessor, valueAccessor, spanItemFactory, valueMerger, skipZeroSpans, _nonce);
		}

		public Enumerator GetEnumerator()
		{
			VerifyNonce();

			return new Enumerator(_implementation, _nonce);
		}

		private void VerifyNonce()
		{
			if (_nonce == 0)
				throw new InvalidOperationException("Disposed");
		}

		public IEnumerable<TItem> AsEnumerable()
		{
			VerifyNonce();

			return new Enumerable(_implementation, _nonce);
		}

		private class Enumerable : IEnumerable<TItem>
		{
			private readonly Implementation _implementation;
			private readonly long _nonce;

			public Enumerable(Implementation implementation, long nonce)
			{
				_nonce = nonce;
				_implementation = implementation;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				_implementation.VerifyNonce(_nonce);

				return new Enumerator(_implementation, _nonce);
			}

			IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
			{
				_implementation.VerifyNonce(_nonce);

				return new Enumerator(_implementation, _nonce);
			}
		}

		public struct Enumerator : IEnumerator<TItem>
		{
			private Implementation _implementation;
			private readonly long _nonce;
			private int _currentIndex;

			internal Enumerator(object implementation, long nonce)
			{
				_currentIndex = -1;
				_implementation = (Implementation)implementation;
				_nonce = nonce;
			}

			public bool MoveNext()
			{
				_implementation.VerifyNonce(_nonce);

				if (_currentIndex == -1)
				{
					if (_implementation.Count == 0)
						return false;
				}

				_currentIndex++;

				return _currentIndex < _implementation.Count;
			}

			public void Reset()
			{
				_implementation.VerifyNonce(_nonce);

				_currentIndex = -1;
			}

			object IEnumerator.Current => Current;

			public TItem Current
			{
				get
				{
					_implementation.VerifyNonce(_nonce);

					if (_currentIndex < 0 || _currentIndex == _implementation.Count)
						throw new InvalidOperationException();

					return _implementation[_currentIndex];
				}
			}

			void IDisposable.Dispose()
			{
				_implementation.VerifyNonce(_nonce);
				_implementation = null;
			}
		}

		public void Dispose()
		{
			_implementation.Release(_nonce);
			_pool.Return(_implementation);
			_implementation = null;
			_pool = null;
			_nonce = 0;
		}

		private sealed class Implementation
		{
			private readonly SpanComparer _spanComparer;
			private readonly ValueOrderComparer _valueOrderComparer;
			private int _count;
			private PooledList<TItem> _mergedList;
			private long _nonce;
			private PooledList<int> _spanSortedIndexList;
			private PooledList<IndexItemSpanValue> _spanValueItemList;
			private PooledList<int> _valueSortedIndexList;

			public Implementation()
			{
				_spanComparer = new SpanComparer(this);
				_valueOrderComparer = new ValueOrderComparer(this);
			}

			public int Count => _count;

			public TItem this[int index] => _mergedList[index];

			private void AddBoundingJunction(IntSpan span, IntSpan boundingSpan, IntSpan prevSpan, Func<IntSpan, TValue, TItem> spanFactory, TValue boundingValue)
			{
				var prevJunction = prevSpan.IsEmpty ? new IntSpan(boundingSpan.Start, 0) : prevSpan;
				var junction = prevJunction.Junction(span);

				if (junction.IsEmpty == false)
					_mergedList.Add(spanFactory(junction, boundingValue));
			}

			public void Mount(
				IEnumerable<TItem> spanItems,
				TItem boundingItem,
				Func<TItem, IntSpan> spanAccessor,
				Func<TItem, TValue> valueAccessor,
				Func<IntSpan, TValue, TItem> spanFactory,
				Func<IReadOnlyCollection<TValue>, TValue> valueMerger,
				bool skipZeroSpans,
				long nonce)
			{
				var mergeCollection = new MergeCollection(this);

				_nonce = nonce;
				_count = 0;
				_spanValueItemList = PooledList<IndexItemSpanValue>.RentList();
				_mergedList = PooledList<TItem>.RentList();
				_spanSortedIndexList = PooledList<int>.RentList();
				_valueSortedIndexList = PooledList<int>.RentList();

				var boundingSpan = spanAccessor(boundingItem);
				var boundingValue = valueAccessor(boundingItem);
				var addBoundingJunction = boundingSpan.IsEmpty == false;
				var index = 0;
				var prevSpan = IntSpan.Empty;
				var needMerge = false;
				var needSort = false;
				var end = 0;

				foreach (var spanItem in spanItems)
				{
					var span = spanAccessor(spanItem);

					needMerge |= prevSpan.IntersectsWith(span) || prevSpan.Contains(span);
					needSort |= span.Start < end;

					if (addBoundingJunction)
						AddBoundingJunction(span, boundingSpan, prevSpan, spanFactory, boundingValue);

					_mergedList.Add(spanItem);
					_spanValueItemList.Add(new IndexItemSpanValue(index, spanItem, span, valueAccessor(spanItem)));
					_spanSortedIndexList.Add(index);
					_valueSortedIndexList.Add(index);

					if (span.End > end)
						end = span.End;

					prevSpan = span;

					index++;
				}

				_count = _spanValueItemList.Count;

				if (_count == 0)
				{
					if (boundingSpan.IsEmpty == false)
					{
						_mergedList.Add(boundingItem);
						_count = 1;
					}

					return;
				}

				if (needSort)
				{
					_spanSortedIndexList.Sort(0, _spanSortedIndexList.Count, _spanComparer);
					_mergedList.Clear();

					needMerge = false;
					prevSpan = IntSpan.Empty;

					for (var i = 0; i < _count; i++)
					{
						var spanValueItem = _spanValueItemList[_spanSortedIndexList[i]];
						var spanItem = spanValueItem.Item;
						var span = spanValueItem.Span;

						needMerge |= prevSpan.IntersectsWith(span) || prevSpan.Contains(span);

						if (needMerge)
							break;
						
						if (addBoundingJunction)
							AddBoundingJunction(span, boundingSpan, prevSpan, spanFactory, boundingValue);

						_mergedList.Add(spanItem);

						prevSpan = span;
					}

					if (boundingSpan.IsEmpty == false)
						AddBoundingJunction(new IntSpan(boundingSpan.End), boundingSpan, prevSpan, spanFactory, boundingValue);
				}
				else if (boundingSpan.IsEmpty == false)
					AddBoundingJunction(new IntSpan(boundingSpan.End), boundingSpan, prevSpan, spanFactory, boundingValue);

				if (needMerge == false)
				{
					_count = _mergedList.Count;

					return;
				}

				_count = _spanValueItemList.Count;
				_mergedList.Clear();

				// ReSharper disable once UseIndexFromEndExpression
				_spanValueItemList.Add(new IndexItemSpanValue(index, default, new IntSpan(end, 0), default));
				_spanSortedIndexList.Add(index);

				var currentIndex = 0;
				var resortIndex = -1;
				var prevMergedSpan = IntSpan.Empty;

				while (currentIndex < _count)
				{
					var nextIndex = resortIndex == -1 ? currentIndex + 1 : resortIndex + 1;
					var currentSpan = _spanValueItemList[_spanSortedIndexList[currentIndex]].Span;
					var currentSpanStart = currentSpan.Start;

					while (nextIndex < _count && _spanValueItemList[_spanSortedIndexList[nextIndex]].Span.Start == currentSpanStart)
						nextIndex++;

					if (resortIndex != -1)
					{
						_spanSortedIndexList.Sort(resortIndex, nextIndex - resortIndex, _spanComparer);

						currentIndex = resortIndex;
						currentSpan = _spanValueItemList[_spanSortedIndexList[currentIndex]].Span;
						currentSpanStart = currentSpan.Start;

						resortIndex = -1;
					}

					var nextSpan = _spanValueItemList[_spanSortedIndexList[nextIndex]].Span;
					var spanEnd = _spanValueItemList[_spanSortedIndexList[nextIndex - 1]].Span.End;
					var split = nextSpan.Start < spanEnd ? nextSpan.Start : spanEnd;

					var mergeSpanStart = 0;
					var mergeSpanLength = split - currentSpanStart;

					while (currentIndex != nextIndex)
					{
						currentSpan = _spanValueItemList[_spanSortedIndexList[currentIndex]].Span;

						var sliceLength = Math.Min(currentSpan.Length, mergeSpanLength) - mergeSpanStart;

						if (sliceLength > 0 || skipZeroSpans == false)
						{
							var mergedSpan = currentSpan.Slice(mergeSpanStart, sliceLength);
							var mergedValue = _spanValueItemList[_spanSortedIndexList[currentIndex]].Value;

							if (nextIndex - currentIndex > 1)
							{
								for (var i = currentIndex; i < nextIndex; i++)
									_valueSortedIndexList[i] = _spanValueItemList[_spanSortedIndexList[i]].Index;

								_valueSortedIndexList.Sort(currentIndex, nextIndex - currentIndex, _valueOrderComparer);

								mergeCollection.Mount(currentIndex, nextIndex - currentIndex);

								mergedValue = valueMerger(mergeCollection);
							}

							mergeSpanStart += mergedSpan.Length;

							if (boundingSpan.IsEmpty == false)
								AddBoundingJunction(mergedSpan, boundingSpan, prevMergedSpan, spanFactory, boundingValue);

							_mergedList.Add(spanFactory(mergedSpan, mergedValue));

							prevMergedSpan = mergedSpan;
						}

						if (currentSpan.End > split)
						{
							resortIndex = currentIndex;

							while (currentIndex < nextIndex)
							{
								var t = _spanValueItemList[_spanSortedIndexList[currentIndex]];
								var splitIndex = split - t.Span.Start;

								if (t.Span.Length < splitIndex)
								{
									resortIndex++;
									currentIndex++;

									continue;
								}

								_spanValueItemList[_spanSortedIndexList[currentIndex++]] = new IndexItemSpanValue(t.Index, default, t.Span.Slice(splitIndex), t.Value);
							}
						}
						else
						{
							while (currentIndex < nextIndex && _spanValueItemList[_spanSortedIndexList[currentIndex]].Span.Length == currentSpan.Length)
								currentIndex++;
						}
					}
				}

				if (boundingSpan.IsEmpty == false)
					AddBoundingJunction(new IntSpan(boundingSpan.End), boundingSpan, prevMergedSpan, spanFactory, boundingValue);

				_count = _mergedList.Count;
			}

			public void Release(long nonce)
			{
				VerifyNonce(nonce);

				PooledList<int>.ReturnList(_spanSortedIndexList);
				PooledList<int>.ReturnList(_valueSortedIndexList);
				PooledList<TItem>.ReturnList(_mergedList);
				PooledList<IndexItemSpanValue>.ReturnList(_spanValueItemList);

				_nonce = 0;
				_spanSortedIndexList = null;
				_mergedList = null;
				_spanValueItemList = null;
			}

			internal void VerifyNonce(long nonce)
			{
				if (_nonce != nonce)
					throw new InvalidOperationException("Invalid nonce");
			}

			private sealed class SpanComparer : IComparer<int>
			{
				private readonly Implementation _implementation;

				public SpanComparer(Implementation implementation)
				{
					_implementation = implementation;
				}

				public int Compare(int x, int y)
				{
					var spanX = _implementation._spanValueItemList[x].Span;
					var spanY = _implementation._spanValueItemList[y].Span;
					var startCompare = spanX.Start.CompareTo(spanY.Start);

					return startCompare == 0 ? spanX.Length.CompareTo(spanY.Length) : startCompare;
				}
			}

			private sealed class ValueOrderComparer : IComparer<int>
			{
				private readonly Implementation _implementation;

				public ValueOrderComparer(Implementation implementation)
				{
					_implementation = implementation;
				}

				public int Compare(int x, int y)
				{
					var orderX = _implementation._spanValueItemList[x].Index;
					var orderY = _implementation._spanValueItemList[y].Index;

					return orderX.CompareTo(orderY);
				}
			}

			private readonly struct IndexItemSpanValue
			{
				public IndexItemSpanValue(int index, TItem item, IntSpan span, TValue value)
				{
					Index = index;
					Item = item;
					Span = span;
					Value = value;
				}

				public readonly TItem Item;
				public readonly TValue Value;
				public readonly IntSpan Span;
				public readonly int Index;

				public override string ToString()
				{
					return $"[{Span.Start}:{Span.End}) - {Value}";
				}
			}

			private sealed class MergeCollection : IReadOnlyList<TValue>
			{
				private readonly ReadOnlyListEnumeratorPool<TValue> _enumeratorPool;
				private readonly Implementation _implementation;
				private int _length;
				private int _start;

				public MergeCollection(Implementation implementation)
				{
					_implementation = implementation;
					_enumeratorPool = ReadOnlyListEnumeratorPool<TValue>.Shared;
				}

				public void Mount(int start, int length)
				{
					_start = start;
					_length = length;
				}

				public IEnumerator<TValue> GetEnumerator()
				{
					return _enumeratorPool.Rent(this);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				public int Count => _length;

				public TValue this[int index] => _implementation._spanValueItemList[_implementation._valueSortedIndexList[_start + index]].Value;
			}
		}
	}
}