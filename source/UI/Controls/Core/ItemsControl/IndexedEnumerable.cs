// <copyright file="IndexedEnumerable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
#if !SILVERLIGHT
using System.Windows.Data;

#endif

namespace Zaaml.UI.Controls.Core
{
	internal sealed class IndexedEnumerable : IEnumerable
	{
		#region Static Fields and Constants

		public static readonly IndexedEnumerable Empty = new IndexedEnumerable(new object[0]);

		#endregion

		#region Fields

		private int _cachedCount = -1;
		private int _cachedIndex = -1;
		private bool? _cachedIsEmpty;
		private object _cachedItem;
		private int _cachedVersion = -1;
		private IEnumerator _changeTracker;
		private IEnumerator _enumerator;
		private int _enumeratorVersion;
		private PropertyInfo _reflectedCount;
		private MethodInfo _reflectedIndexOf;
		private PropertyInfo _reflectedItemAt;

		#endregion

		#region Ctors

		internal IndexedEnumerable(IEnumerable collection)
			: this(collection, null)
		{
		}

		internal IndexedEnumerable(IEnumerable collection, Predicate<object> filterCallback)
		{
			FilterCallback = filterCallback;

			SetCollection(collection);

			if (List != null)
				return;

			if (collection is INotifyCollectionChanged icc)
			{
#if SILVERLIGHT
				// TODO: Implement weak event pattern
	      icc.CollectionChanged += OnCollectionChanged;
#else
				CollectionChangedEventManager.AddHandler(icc, OnCollectionChanged);
#endif
			}
		}

		#endregion

		#region Properties

		internal ICollection Collection { get; private set; }

#if !SILVERLIGHT
		internal CollectionView CollectionView { get; private set; }
#endif

		internal int Count
		{
			get
			{
				if (GetNativeCount(out var count))
					return count;

				EnsureCacheCurrent();

				if (_cachedCount >= 0)
					return _cachedCount;

				count = 0;

				foreach (var unused in this)
					count++;

				_cachedCount = count;
				_cachedIsEmpty = _cachedCount == 0;

				return count;
			}
		}

		internal IEnumerable Enumerable { get; private set; }

		private Predicate<object> FilterCallback { get; set; }

		internal bool IsEmpty
		{
			get
			{
				if (GetNativeIsEmpty(out var isEmpty))
					return isEmpty;

				if (_cachedIsEmpty.HasValue)
					return _cachedIsEmpty.Value;

				var ie = GetEnumerator();

				_cachedIsEmpty = !ie.MoveNext();

				var d = ie as IDisposable;

				d?.Dispose();

				if (_cachedIsEmpty.Value)
					_cachedCount = 0;

				return _cachedIsEmpty.Value;
			}
		}

		internal object this[int index]
		{
			get
			{
				if (GetNativeItemAt(index, out var value))
					return value;

				if (index < 0)
					throw new ArgumentOutOfRangeException(nameof(index));

				var moveBy = index - _cachedIndex;

				if (moveBy < 0)
				{
					UseNewEnumerator();

					moveBy = index + 1;
				}

				if (EnsureCacheCurrent())
				{
					if (index == _cachedIndex)
						return _cachedItem;
				}
				else
					moveBy = index + 1;

				while (moveBy > 0 && _enumerator.MoveNext())
					moveBy--;

				if (moveBy != 0)
					throw new ArgumentOutOfRangeException(nameof(index));

				CacheCurrentItem(index, _enumerator.Current);

				return _cachedItem;
			}
		}

		internal IList List { get; private set; }

		#endregion

		#region  Methods

		private void CacheCurrentItem(int index, object item)
		{
			_cachedIndex = index;
			_cachedItem = item;
			_cachedVersion = _enumeratorVersion;
		}

		private void ClearAllCaches()
		{
			_cachedItem = null;
			_cachedIndex = -1;
			_cachedCount = -1;
		}

		internal static void CopyTo(IEnumerable collection, Array array, int index)
		{
			var ic = collection as ICollection;

			if (ic != null)
				ic.CopyTo(array, index);
			else
			{
				var list = (IList) array;

				foreach (var item in collection)
				{
					if (index < array.Length)
					{
						list[index] = item;
						++index;
					}
					else
					{
						//throw new ArgumentException(SR.Get(SRID.CopyToNotEnoughSpace), nameof(index));
						throw new InvalidOperationException();
					}
				}
			}
		}

		private void DisposeEnumerator(ref IEnumerator ie)
		{
			var d = ie as IDisposable;
			d?.Dispose();

			ie = null;
		}

		private bool EnsureCacheCurrent()
		{
			var version = EnsureEnumerator();

			if (version != _cachedVersion)
			{
				ClearAllCaches();

				_cachedVersion = version;
			}

			var isCacheCurrent = version == _cachedVersion && _cachedIndex >= 0;

			return isCacheCurrent;
		}

		private int EnsureEnumerator()
		{
			if (_enumerator == null)
			{
				UseNewEnumerator();
			}
			else
			{
				try
				{
					_changeTracker.MoveNext();
				}
				catch (InvalidOperationException)
				{
					UseNewEnumerator();
				}
			}

			return _enumeratorVersion;
		}

		private bool GetNativeCount(out int value)
		{
			var isNativeValue = false;

			value = -1;

			if (Collection != null)
			{
				value = Collection.Count;

				isNativeValue = true;
			}
#if !SILVERLIGHT
			else if (CollectionView != null)
			{
				value = CollectionView.Count;

				isNativeValue = true;
			}
#endif
			else if (_reflectedCount != null)
			{
				try
				{
					value = (int) _reflectedCount.GetValue(Enumerable, null);
					isNativeValue = true;
				}
				catch (MethodAccessException)
				{
					_reflectedCount = null;
					isNativeValue = false;
				}
			}

			return isNativeValue;
		}

		private bool GetNativeIndexOf(object item, out int value)
		{
			var isNativeValue = false;

			value = -1;

			if ((List != null) && (FilterCallback == null))
			{
				value = List.IndexOf(item);
				isNativeValue = true;
			}
#if !SILVERLIGHT
			else if (CollectionView != null)
			{
				value = CollectionView.IndexOf(item);
				isNativeValue = true;
			}
#endif
			else if (_reflectedIndexOf != null)
			{
				try
				{
					value = (int) _reflectedIndexOf.Invoke(Enumerable, new[] { item });
					isNativeValue = true;
				}
				catch (MethodAccessException)
				{
					_reflectedIndexOf = null;
					isNativeValue = false;
				}
			}

			return isNativeValue;
		}

		private bool GetNativeIsEmpty(out bool isEmpty)
		{
			var isNativeValue = false;

			isEmpty = true;

			if (Collection != null)
			{
				isEmpty = (Collection.Count == 0);
				isNativeValue = true;
			}
#if !SILVERLIGHT
			else if (CollectionView != null)
			{
				isEmpty = CollectionView.IsEmpty;
				isNativeValue = true;
			}
#endif
			else if (_reflectedCount != null)
			{
				try
				{
					isEmpty = ((int) _reflectedCount.GetValue(Enumerable, null) == 0);
					isNativeValue = true;
				}
				catch (MethodAccessException)
				{
					_reflectedCount = null;
					isNativeValue = false;
				}
			}

			return isNativeValue;
		}

		private bool GetNativeItemAt(int index, out object value)
		{
			var isNativeValue = false;

			value = null;

			if (List != null)
			{
				value = List[index];
				isNativeValue = true;
			}
#if !SILVERLIGHT
			else if (CollectionView != null)
			{
				value = CollectionView.GetItemAt(index);
				isNativeValue = true;
			}
#endif
			else if (_reflectedItemAt != null)
			{
				try
				{
					value = _reflectedItemAt.GetValue(Enumerable, new object[] { index });
					isNativeValue = true;
				}
				catch (MethodAccessException)
				{
					_reflectedItemAt = null;
					isNativeValue = false;
				}
			}

			return isNativeValue;
		}


		internal int IndexOf(object item)
		{
			int index;

			if (GetNativeIndexOf(item, out index))
				return index;

			if (EnsureCacheCurrent())
			{
				if (item == _cachedItem)
					return _cachedIndex;
			}

			index = -1;

			if (_cachedIndex >= 0)
				UseNewEnumerator();

			var i = 0;

			while (_enumerator.MoveNext())
			{
				if (Equals(_enumerator.Current, item))
				{
					index = i;
					break;
				}

				++i;
			}

			if (index >= 0)
				CacheCurrentItem(index, _enumerator.Current);
			else
			{
				ClearAllCaches();
				DisposeEnumerator(ref _enumerator);
			}

			return index;
		}


		internal void Invalidate()
		{
			ClearAllCaches();

			if (List == null)
			{
				var icc = Enumerable as INotifyCollectionChanged;

				if (icc != null)
				{
#if SILVERLIGHT
		      // TODO: Implement weak event pattern
		      icc.CollectionChanged -= OnCollectionChanged;
#else
					CollectionChangedEventManager.RemoveHandler(icc, OnCollectionChanged);
#endif
				}
			}

			Enumerable = null;

			DisposeEnumerator(ref _enumerator);
			DisposeEnumerator(ref _changeTracker);

			Collection = null;
			List = null;
			FilterCallback = null;
		}

		private void InvalidateEnumerator()
		{
			// if _enumeratorVersion exceeds MaxValue, then it
			// will roll back to MinValue, and continue on from there.
			unchecked
			{
				++_enumeratorVersion;
			}

			DisposeEnumerator(ref _enumerator);
			ClearAllCaches();
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidateEnumerator();
		}

		private void SetCollection(IEnumerable collection)
		{
			Enumerable = collection;
			Collection = collection as ICollection;
			List = collection as IList;
#if !SILVERLIGHT
			CollectionView = collection as CollectionView;

			if (List == null && CollectionView == null)
#else
      if (List == null)
#endif
			{
				var srcType = collection.GetType();
				var mi = srcType.GetMethod("IndexOf", new[] { typeof(object) });

				if (mi != null && mi.ReturnType == typeof(int))
					_reflectedIndexOf = mi;

				var defaultMembers = srcType.GetDefaultMembers();

				for (var i = 0; i <= defaultMembers.Length - 1; i++)
				{
					var pi = defaultMembers[i] as PropertyInfo;

					if (pi == null)
						continue;

					var indexerParameters = pi.GetIndexParameters();

					if (indexerParameters.Length != 1)
						continue;

					if (!indexerParameters[0].ParameterType.IsAssignableFrom(typeof(int)))
						continue;

					_reflectedItemAt = pi;

					break;
				}

				if (Collection == null)
				{
					var pi = srcType.GetProperty("Count", typeof(int));

					if (pi != null)
					{
						_reflectedCount = pi;
					}
				}
			}
		}

		private void UseNewEnumerator()
		{
			unchecked
			{
				++_enumeratorVersion;
			}

			DisposeEnumerator(ref _changeTracker);

			_changeTracker = Enumerable.GetEnumerator();

			DisposeEnumerator(ref _enumerator);

			_enumerator = GetEnumerator();
			_cachedIndex = -1;
			_cachedItem = null;
		}

		#endregion

		#region Interface Implementations

		#region IEnumerable

		public IEnumerator GetEnumerator()
		{
			return new FilteredEnumerator(this, Enumerable, FilterCallback);
		}

		#endregion

		#endregion

		#region  Nested Types

		private class FilteredEnumerator : IEnumerator, IDisposable
		{
			#region Fields

			private readonly IEnumerable _enumerable;
			private readonly Predicate<object> _filterCallback;
			private readonly IndexedEnumerable _indexedEnumerable;
			private IEnumerator _enumerator;

			#endregion

			#region Ctors

			public FilteredEnumerator(IndexedEnumerable indexedEnumerable, IEnumerable enumerable, Predicate<object> filterCallback)
			{
				_enumerable = enumerable;
				_enumerator = _enumerable.GetEnumerator();
				_filterCallback = filterCallback;
				_indexedEnumerable = indexedEnumerable;
			}

			#endregion

			#region Interface Implementations

			#region IDisposable

			public void Dispose()
			{
				var d = _enumerator as IDisposable;

				d?.Dispose();

				_enumerator = null;
			}

			#endregion

			#region IEnumerator

			void IEnumerator.Reset()
			{
				if (_indexedEnumerable.Enumerable == null)
				{
					//throw new InvalidOperationException(SR.Get(SRID.EnumeratorVersionChanged));
					throw new InvalidOperationException();
				}

				Dispose();

				_enumerator = _enumerable.GetEnumerator();
			}

			bool IEnumerator.MoveNext()
			{
				bool returnValue;

				if (_indexedEnumerable.Enumerable == null)
				{
					//throw new InvalidOperationException(SR.Get(SRID.EnumeratorVersionChanged));
					throw new InvalidOperationException();
				}

				if (_filterCallback == null)
				{
					returnValue = _enumerator.MoveNext();
				}
				else
				{
					while ((returnValue = _enumerator.MoveNext()) && !_filterCallback(_enumerator.Current))
					{
					}
				}

				return returnValue;
			}

			object IEnumerator.Current => _enumerator.Current;

			#endregion

			#endregion
		}

		#endregion
	}
}