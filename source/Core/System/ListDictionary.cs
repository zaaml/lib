// <copyright file="ListDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading;

// ReSharper disable once CheckNamespace
namespace System.Collections.Specialized
{
  public class ListDictionary : IDictionary
  {
    #region Fields

    private readonly IComparer _comparer;
    private DictionaryNode _head;

    private object _syncRoot;
    private int _version;

    #endregion

    #region Ctors

    public ListDictionary()
    {
    }


    public ListDictionary(IComparer comparer)
    {
      _comparer = comparer;
    }

    #endregion

    #region Interface Implementations

    #region ICollection

    public int Count { get; private set; }

    public bool IsSynchronized => false;

    public object SyncRoot
    {
      get
      {
        if (_syncRoot == null)
          Interlocked.CompareExchange(ref _syncRoot, new object(), null);

        return _syncRoot;
      }
    }


    public void CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException(nameof(array));

      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof(index));

      if (array.Length - index < Count)
        throw new ArgumentException("InsufficientSpace");

      for (var node = _head; node != null; node = node.Next)
      {
        array.SetValue(new DictionaryEntry(node.Key, node.Value), index);
        index++;
      }
    }

    #endregion

    #region IDictionary

    public object this[object key]
    {
      get
      {
        if (key == null)
          throw new ArgumentNullException(nameof(key));

        var node = _head;
        if (_comparer == null)
        {
          while (node != null)
          {
            var oldKey = node.Key;
            if (oldKey != null && oldKey.Equals(key))
            {
              return node.Value;
            }
            node = node.Next;
          }
        }
        else
        {
          while (node != null)
          {
            var oldKey = node.Key;
            if (oldKey != null && _comparer.Compare(oldKey, key) == 0)
            {
              return node.Value;
            }
            node = node.Next;
          }
        }
        return null;
      }
      set
      {
        if (key == null)
          throw new ArgumentNullException(nameof(key));

        _version++;
        DictionaryNode last = null;
        DictionaryNode node;
        for (node = _head; node != null; node = node.Next)
        {
          var oldKey = node.Key;
          if ((_comparer == null) ? oldKey.Equals(key) : _comparer.Compare(oldKey, key) == 0)
          {
            break;
          }
          last = node;
        }
        if (node != null)
        {
          // Found it
          node.Value = value;
          return;
        }
        // Not found, so add a new one
        var newNode = new DictionaryNode
        {
          Key = key,
          Value = value
        };
        if (last != null)
          last.Next = newNode;
        else
          _head = newNode;
        Count++;
      }
    }

    public ICollection Keys => new NodeKeyValueCollection(this, true);

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public ICollection Values => new NodeKeyValueCollection(this, false);

    public void Add(object key, object value)
    {
      if (key == null)
      {
        throw new ArgumentNullException(nameof(key));
      }
      _version++;
      DictionaryNode last = null;
      DictionaryNode node;
      for (node = _head; node != null; node = node.Next)
      {
        var oldKey = node.Key;
        if (_comparer == null ? oldKey.Equals(key) : _comparer.Compare(oldKey, key) == 0)
          throw new ArgumentException("Duplicate key");

        last = node;
      }
      var newNode = new DictionaryNode
      {
        Key = key,
        Value = value
      };
      if (last != null)
        last.Next = newNode;
      else
        _head = newNode;
      Count++;
    }

    public void Clear()
    {
      Count = 0;
      _head = null;
      _version++;
    }

    public bool Contains(object key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));

      for (var node = _head; node != null; node = node.Next)
      {
        var oldKey = node.Key;
        if (_comparer == null ? oldKey.Equals(key) : _comparer.Compare(oldKey, key) == 0)
          return true;
      }

      return false;
    }

    public IDictionaryEnumerator GetEnumerator()
    {
      return new NodeEnumerator(this);
    }

    public void Remove(object key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));

      _version++;
      DictionaryNode last = null;
      DictionaryNode node;

      for (node = _head; node != null; node = node.Next)
      {
        var oldKey = node.Key;
        if (_comparer == null ? oldKey.Equals(key) : _comparer.Compare(oldKey, key) == 0)
          break;

        last = node;
      }

      if (node == null)
        return;
      if (node == _head)
        _head = node.Next;
      else
        last.Next = node.Next;

      Count--;
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => new NodeEnumerator(this);

    #endregion

    #endregion

    #region  Nested Types

    private class NodeEnumerator : IDictionaryEnumerator
    {
      #region Fields

      private readonly ListDictionary _list;
      private readonly int _version;
      private DictionaryNode _current;
      private bool _start;

      #endregion

      #region Ctors

      public NodeEnumerator(ListDictionary list)
      {
        _list = list;
        _version = list._version;
        _start = true;
        _current = null;
      }

      #endregion

      #region Interface Implementations

      #region IDictionaryEnumerator

      public DictionaryEntry Entry
      {
        get
        {
          if (_current == null)
            throw new InvalidOperationException();

          return new DictionaryEntry(_current.Key, _current.Value);
        }
      }

      public object Key
      {
        get
        {
          if (_current == null)
            throw new InvalidOperationException();

          return _current.Key;
        }
      }

      public object Value
      {
        get
        {
          if (_current == null)
            throw new InvalidOperationException();

          return _current.Value;
        }
      }

      #endregion

      #region IEnumerator

      public object Current => Entry;

      public bool MoveNext()
      {
        if (_version != _list._version)
          throw new InvalidOperationException();

        if (_start)
        {
          _current = _list._head;
          _start = false;
        }
        else
          _current = _current?.Next;

        return _current != null;
      }

      public void Reset()
      {
        if (_version != _list._version)
          throw new InvalidOperationException();

        _start = true;
        _current = null;
      }

      #endregion

      #endregion
    }


    private class NodeKeyValueCollection : ICollection
    {
      #region Fields

      private readonly bool _isKeys;
      private readonly ListDictionary _list;

      #endregion

      #region Ctors

      public NodeKeyValueCollection(ListDictionary list, bool isKeys)
      {
        _list = list;
        _isKeys = isKeys;
      }

      #endregion

      #region Interface Implementations

      #region ICollection

      void ICollection.CopyTo(Array array, int index)
      {
        if (array == null)
          throw new ArgumentNullException(nameof(array));

        if (index < 0)
          throw new ArgumentOutOfRangeException(nameof(index));

        for (var node = _list._head; node != null; node = node.Next)
        {
          array.SetValue(_isKeys ? node.Key : node.Value, index);
          index++;
        }
      }

      int ICollection.Count
      {
        get
        {
          var count = 0;
          for (var node = _list._head; node != null; node = node.Next)
            count++;

          return count;
        }
      }

      bool ICollection.IsSynchronized => false;

      object ICollection.SyncRoot => _list.SyncRoot;

      #endregion

      #region IEnumerable

      IEnumerator IEnumerable.GetEnumerator()
      {
        return new NodeKeyValueEnumerator(_list, _isKeys);
      }

      #endregion

      #endregion

      #region  Nested Types

      private class NodeKeyValueEnumerator : IEnumerator
      {
        #region Fields

        private readonly bool _isKeys;
        private readonly ListDictionary _list;
        private readonly int _version;
        private DictionaryNode _current;
        private bool _start;

        #endregion

        #region Ctors

        public NodeKeyValueEnumerator(ListDictionary list, bool isKeys)
        {
          _list = list;
          _isKeys = isKeys;
          _version = list._version;
          _start = true;
          _current = null;
        }

        #endregion

        #region Interface Implementations

        #region IEnumerator

        public object Current
        {
          get
          {
            if (_current == null)
              throw new InvalidOperationException();

            return _isKeys ? _current.Key : _current.Value;
          }
        }

        public bool MoveNext()
        {
          if (_version != _list._version)
            throw new InvalidOperationException();

          if (_start)
          {
            _current = _list._head;
            _start = false;
          }
          else
            _current = _current?.Next;

          return (_current != null);
        }

        public void Reset()
        {
          if (_version != _list._version)
            throw new InvalidOperationException();

          _start = true;
          _current = null;
        }

        #endregion

        #endregion
      }

      #endregion
    }


    private class DictionaryNode
    {
      #region Fields

      public object Key;
      public DictionaryNode Next;
      public object Value;

      #endregion
    }

    #endregion
  }
}
