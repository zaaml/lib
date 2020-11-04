// <copyright file="HybridDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Collections.Specialized
{
	internal class HybridDictionary : IDictionary
  {
    #region Static Fields and Constants

    private const int CutoverPoint = 9;
    private const int InitialHashtableSize = 13;
    private const int FixedSizeCutoverPoint = 6;

    #endregion

    #region Fields

    private readonly bool _caseInsensitive;
    private Dictionary<object, object> _hashtable;

    private ListDictionary _list;

    #endregion

    #region Ctors

    public HybridDictionary()
    {
    }

    public HybridDictionary(int initialSize) : this(initialSize, false)
    {
    }

    public HybridDictionary(bool caseInsensitive)
    {
      _caseInsensitive = caseInsensitive;
    }

    public HybridDictionary(int initialSize, bool caseInsensitive)
    {
      _caseInsensitive = caseInsensitive;
      if (initialSize >= FixedSizeCutoverPoint)
        _hashtable = new Dictionary<object, object>(initialSize);
    }

    #endregion

    #region Properties

    private ListDictionary List => _list ??= new ListDictionary(_caseInsensitive ? StringComparer.OrdinalIgnoreCase : null);

    #endregion

    #region  Methods

    private void ChangeOver()
    {
      var en = _list.GetEnumerator();
      var newTable = new Dictionary<object, object>(InitialHashtableSize);

      while (en.MoveNext())
        newTable.Add(en.Key, en.Value);

      _hashtable = newTable;
      _list = null;
    }

    #endregion

    #region Interface Implementations

    #region ICollection

    public int Count
    {
      get
      {
        var cachedList = _list;

        if (_hashtable != null)
          return _hashtable.Count;

        return cachedList?.Count ?? 0;
      }
    }

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public void CopyTo(Array array, int index)
    {
      throw new NotSupportedException();
    }

    #endregion

    #region IDictionary

    public object this[object key]
    {
      get
      {
        var cachedList = _list;

        if (_hashtable != null)
	        return _hashtable.TryGetValue(key, out var value) ? value : null;

        if (cachedList != null)
          return cachedList[key];

        if (key == null)
          throw new ArgumentNullException(nameof(key));

        return null;
      }
      set
      {
        if (_hashtable != null)
          _hashtable[key] = value;
        else if (_list != null)
        {
          if (_list.Count >= CutoverPoint - 1)
          {
            ChangeOver();
            _hashtable[key] = value;
          }
          else
            _list[key] = value;
        }
        else
          _list = new ListDictionary(_caseInsensitive ? StringComparer.OrdinalIgnoreCase : null) {[key] = value};
      }
    }

    public ICollection Keys => _hashtable?.Keys ?? List.Keys;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public ICollection Values => _hashtable?.Values ?? List.Values;

    public void Add(object key, object value)
    {
      if (_hashtable != null)
        _hashtable.Add(key, value);
      else
      {
        if (_list == null)
          _list = new ListDictionary(_caseInsensitive ? StringComparer.OrdinalIgnoreCase : null) {{key, value}};
        else
        {
          if (_list.Count + 1 >= CutoverPoint)
          {
            ChangeOver();

            _hashtable.Add(key, value);
          }
          else
            _list.Add(key, value);
        }
      }
    }

    public void Clear()
    {
      if (_hashtable != null)
      {
        var cachedHashtable = _hashtable;

        _hashtable = null;
        cachedHashtable.Clear();
      }

      if (_list == null) 
	      return;

      var cachedList = _list;

      _list = null;
      cachedList.Clear();
    }

    public bool Contains(object key)
    {
      var cachedList = _list;

      if (_hashtable != null)
        return _hashtable.ContainsKey(key);

      if (cachedList != null)
        return cachedList.Contains(key);

      if (key == null)
        throw new ArgumentNullException(nameof(key));

      return false;
    }

    public IDictionaryEnumerator GetEnumerator()
    {
      if (_hashtable != null)
        return _hashtable.GetEnumerator();

      if (_list == null)
        _list = new ListDictionary(_caseInsensitive ? StringComparer.OrdinalIgnoreCase : null);

      return _list.GetEnumerator();
    }

    public void Remove(object key)
    {
      if (_hashtable != null)
        _hashtable.Remove(key);
      else if (_list != null)
        _list.Remove(key);
      else
      {
        if (key == null)
          throw new ArgumentNullException(nameof(key));
      }
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      if (_hashtable != null)
        return _hashtable.GetEnumerator();

      if (_list == null)
        _list = new ListDictionary(_caseInsensitive ? StringComparer.OrdinalIgnoreCase : null);

      return _list.GetEnumerator();
    }

    #endregion

    #endregion
  }
}
