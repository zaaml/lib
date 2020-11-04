// <copyright file="TreeEnumerator.Base.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Core.Trees
{
	internal abstract class TreeEnumerator<T> : ITreeEnumerator<T>
	{
		private static readonly TreeStack EnumerationFinished = new TreeStack();
		protected T CurrentNode;
		protected bool HasCurrentNode;

		// ReSharper disable once InconsistentNaming
		private RootEnumerator Root;
		internal TreeStack Stack;
		protected ITreeEnumeratorAdvisor<T> TreeAdvisor;
		internal int Version;

		protected TreeEnumerator(T root, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			if (root == null)
				throw new ArgumentNullException(nameof(root));

			Root = new RootEnumerator(root);
			TreeAdvisor = treeAdvisor ?? throw new ArgumentNullException(nameof(treeAdvisor));
		}

		protected TreeEnumerator(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			Root = new RootEnumerator(treeItems.GetEnumerator());
			TreeAdvisor = treeAdvisor ?? throw new ArgumentNullException(nameof(treeAdvisor));
		}

		protected TreeEnumerator(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			Root = new RootEnumerator(treeItemsEnumerator);
			TreeAdvisor = treeAdvisor ?? throw new ArgumentNullException(nameof(treeAdvisor));
		}

		internal abstract bool AncestorsIncludesSelf { get; }

		private bool IsEnumerationFinished => ReferenceEquals(Stack, EnumerationFinished);

		protected abstract bool MoveNextCore();

		protected bool ProceedNextRootItem()
		{
			HasCurrentNode = false;

			T nextRoot;

			if (Root.MoveNext())
				nextRoot = Root.Current;
			else
				return false;

			var rootNodeEnumerator = NodeEnumerator.Create(nextRoot, TreeAdvisor);

			if (rootNodeEnumerator.MoveNext())
			{
				Stack ??= new TreeStack();
				Stack.Push(rootNodeEnumerator);

				return true;
			}

			HasCurrentNode = true;
			CurrentNode = nextRoot;

			return true;
		}

		public void Dispose()
		{
			if (TreeAdvisor == null)
				return;

			TreeAdvisor = null;
			Root = RootEnumerator.Empty;
			Stack?.Clear();
		}

		public void Reset()
		{
			Stack = null;
			CurrentNode = default;
			HasCurrentNode = false;
		}

		public bool MoveNext()
		{
			if (IsEnumerationFinished)
				return false;

			var next = MoveNextCore();

			Version++;

			if (next == false)
				Stack = EnumerationFinished;

			return next;
		}

		object IEnumerator.Current => Current;

		public T Current => HasCurrentNode && IsEnumerationFinished == false ? CurrentNode : default;

		public AncestorsEnumerator<T> CurrentAncestors => new AncestorsEnumerator<T>(this);

		private struct RootEnumerator
		{
			private IEnumerator<T> _enumerator;
			private readonly T _root;
			private static readonly IEnumerator<T> EmptyEnumerator = Enumerable.Empty<T>().GetEnumerator();
			public static readonly RootEnumerator Empty = new RootEnumerator(EmptyEnumerator);

			public RootEnumerator(IEnumerator<T> enumerator) : this()
			{
				_enumerator = enumerator;
			}

			public RootEnumerator(T root) : this()
			{
				_root = root;
			}

			public bool MoveNext()
			{
				if (ReferenceEquals(_enumerator, EmptyEnumerator))
					return false;

				if (_enumerator != null)
					return _enumerator.MoveNext();

				_enumerator = EmptyEnumerator;

				return true;
			}

			public T Current => _enumerator != null && ReferenceEquals(_enumerator, EmptyEnumerator) == false ? _enumerator.Current : _root;
		}

		internal struct NodeEnumerator
		{
			private int State { get; set; }

			private NodeEnumerator(T node, IEnumerator<T> enumerator)
			{
				Node = node;
				Enumerator = enumerator;
				State = 4;
			}

			public readonly T Node;

			private IEnumerator<T> Enumerator { get; }

			public bool IsVisited => (State & 1) != 0;

			public bool HasNext => (State & 2) == 0;

			public bool HasCurrent => (State & 4) == 0;

			public T CurrentChild => HasCurrent ? Enumerator.Current : default;

			public void Visit()
			{
				State |= 1;
			}

			public bool MoveNext()
			{
				if (HasNext == false)
					return false;

				var hasNext = Enumerator.MoveNext();

				if (hasNext == false)
				{
					State |= 2;

					if (HasCurrent)
						State |= 4;
				}
				else
					State &= ~4;

				return hasNext;
			}

			public static NodeEnumerator Create(T node, ITreeEnumeratorAdvisor<T> advisor)
			{
				return new NodeEnumerator(node, advisor.GetChildren(node));
			}

			public override string ToString()
			{
				return Node?.ToString() ?? "";
			}
		}

		internal sealed class TreeStack
		{
			private const int DefaultCapacity = 4;
			private static readonly NodeEnumerator[] EmptyArray = new NodeEnumerator[0];

			private NodeEnumerator[] _array;
			private int _count;

			public TreeStack()
			{
				_array = EmptyArray;
				_count = 0;
			}

			public int Count => _count;

			public NodeEnumerator this[int index]
			{
				get
				{
					if (_count == 0)
						throw new Exception("Stack is empty");

					if (index >= _count)
						throw new ArgumentOutOfRangeException();

					return _array[index];
				}
			}

			public void Clear()
			{
				Array.Clear(_array, 0, _count);
				_count = 0;
			}

			public bool MoveNextPeakNode(out NodeEnumerator enumerator)
			{
				var nodeEnumerator = _array[_count - 1];
				var result = nodeEnumerator.MoveNext();

				_array[_count - 1] = nodeEnumerator;

				enumerator = nodeEnumerator;

				return result;
			}

			public NodeEnumerator Peek()
			{
				if (_count == 0)
					throw new Exception("Stack is empty");

				return _array[_count - 1];
			}

			public NodeEnumerator Pop()
			{
				if (_count == 0)
					throw new Exception("Stack is empty");

				var array = _array;
				var num = _count - 1;

				_count = num;

				var index = num;
				var obj = array[index];

				_array[_count] = default;

				return obj;
			}

			public void Push(NodeEnumerator item)
			{
				if (_count == _array.Length)
				{
					var objArray = new NodeEnumerator[_array.Length == 0 ? DefaultCapacity : 2 * _array.Length];

					Array.Copy(_array, 0, objArray, 0, _count);

					_array = objArray;
				}

				var array = _array;
				var size = _count;

				_count = size + 1;

				var index = size;
				var obj = item;

				array[index] = obj;
			}

			public void VisitPeakNode()
			{
				var nodeEnumerator = _array[_count - 1];

				nodeEnumerator.Visit();
				_array[_count - 1] = nodeEnumerator;
			}
		}
	}
}