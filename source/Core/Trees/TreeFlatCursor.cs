// <copyright file="TreeFlatCursor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Trees
{

	internal abstract class TreeFlatCursor<TNode> where TNode : class
	{
		private Data[] _data = new Data[4];
		private int _depth;

		public int Count
		{
			get
			{
				Ensure();

				return FlatCount < 0 ? 0 : FlatCount;
			}
		}

		private TNode CurrentNode => _data[_depth].Node;

		private int FlatCount { get; set; } = -1;

		private int FlatIndex { get; set; }

		private int Version { get; set; }

		protected virtual int CalcFlatCount()
		{
			if (FlatCount < 0)
				return 0;

			var count = 1;

			while (MoveNext()) 
				count++;

			return count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Ensure()
		{
			if (FlatCount != -1)
				return;

			Initialize();
		}

		private void Initialize()
		{
			_depth = 0;

			for (var i = 0; i < _data.Length; i++)
				_data[i] = new Data();

			var child = GetFirstChild(null, out var index);

			if (child == null)
			{
				FlatCount = -2;
				FlatIndex = -1;

				return;
			}

			FlatCount = 0;
			FlatIndex = 0;

			_data[0] = new Data
			{
				Index = index,
				Node = child
			};

			FlatCount = CalcFlatCount();

			if (FlatCount == 0)
				return;

			_depth = 0;

			for (var i = 0; i < _data.Length; i++)
				_data[i] = new Data();

			FlatIndex = 0;

			_data[0] = new Data
			{
				Index = index,
				Node = child
			};
		}

		private void EnsureDataSize(int size)
		{
			ArrayUtils.EnsureArrayLength(ref _data, size, true);
		}

		public int IndexOf(Func<TNode, bool> predicate)
		{
			Ensure();

			if (FlatCount <= 0)
				return -1;

			ResetPosition();

			var index = 0;

			do
			{
				if (predicate(CurrentNode))
					return index;

				index++;
			} while (MoveNext());

			return -1;
		}

		public int IndexOf(TNode treeNode)
		{
			Ensure();

			if (FlatCount <= 0)
				return -1;

			ResetPosition();

			var index = 0;

			do
			{
				if (ReferenceEquals(treeNode, CurrentNode))
					return index;

				index++;
			} while (MoveNext());

			return -1;
		}

		private protected virtual TNode GetFirstChild(TNode parent, out int index)
		{
			var children = GetNodeChildren(parent);

			index = -1;

			if (children == null || children.Count == 0)
				return null;

			index = 0;

			return children[index];
		}

		private protected virtual TNode GetLastChild(TNode parent, out int index)
		{
			var children = GetNodeChildren(parent);

			index = -1;

			if (children == null)
				return null;

			var childrenCount = children.Count;

			if (childrenCount == 0)
				return null;

			index = childrenCount - 1;

			return children[index];
		}

		private protected virtual TNode GetNextSibling(TNode parent, TNode child, ref int childIndex)
		{
			var children = GetNodeChildren(parent);

			Debug.Assert(ReferenceEquals(children[childIndex], child));

			if (childIndex + 1 < children.Count)
			{
				childIndex++;

				return children[childIndex];
			}

			childIndex = -1;

			return null;
		}

		protected abstract IReadOnlyList<TNode> GetNodeChildren(TNode node);

		private protected virtual TNode GetPrevSibling(TNode parent, TNode child, ref int childIndex)
		{
			var children = GetNodeChildren(parent);

			Debug.Assert(ReferenceEquals(children[childIndex], child));

			if (childIndex > 0)
			{
				childIndex--;

				return children[childIndex];
			}

			childIndex = -1;

			return null;
		}

		private void ResetPosition()
		{
			if (FlatCount <= 0)
				return;

			_depth = 0;

			for (var i = 0; i < _data.Length; i++)
				_data[i] = new Data();

			var child = GetFirstChild(null, out var index);

			FlatIndex = 0;

			_data[0] = new Data
			{
				Index = index,
				Node = child
			};
		}

		protected abstract bool IsExpanded(TNode node);

		private bool MoveNext()
		{
			var data = _data[_depth];

			if (data.Node == null)
				return false;

			if (IsExpanded(data.Node))
			{
				var next = GetFirstChild(data.Node, out var index);

				if (next != null)
				{
					_depth++;

					EnsureDataSize(_depth + 1);

					_data[_depth] = new Data
					{
						Index = index,
						Node = next
					};

					FlatIndex++;

					return true;
				}
			}

			while (true)
			{
				var index = data.Index;

				var next = GetNextSibling(_depth == 0 ? null : _data[_depth - 1].Node, data.Node, ref index);

				if (next != null)
				{
					data.Index = index;
					data.Node = next;

					_data[_depth] = data;

					FlatIndex++;

					return true;
				}

				if (_depth == 0)
					return false;

				var hasNext = false;

				for (var depth = _depth; depth >= 0; depth--)
				{
					ref var currentData = ref _data[depth];

					index = currentData.Index;
					next = GetNextSibling(depth == 0 ? null : _data[depth - 1].Node, currentData.Node, ref index);

					if (next == null)
						continue;

					hasNext = true;

					break;
				}

				if (hasNext == false)
					return false;

				_data[_depth] = new Data();
				_depth--;

				data = _data[_depth];
			}
		}

		private bool MovePrev()
		{
			var data = _data[_depth];

			if (data.Node == null)
				return false;

			var index = data.Index;
			var prev = GetPrevSibling(_depth == 0 ? null : _data[_depth - 1].Node, data.Node, ref index);

			if (prev != null)
			{
				data.Index = index;
				data.Node = prev;

				_data[_depth] = data;

				while (IsExpanded(data.Node))
				{
					prev = GetLastChild(data.Node, out index);

					if (prev == null)
						break;

					_depth++;

					EnsureDataSize(_depth + 1);

					data = new Data
					{
						Index = index,
						Node = prev
					};

					_data[_depth] = data;
				}

				FlatIndex--;

				return true;
			}

			if (_depth == 0)
				return false;

			_data[_depth] = new Data();
			_depth--;

			FlatIndex--;

			return true;
		}

		public TNode ElementAt(int flatIndex)
		{
			Ensure();

			if (FlatCount <= 0)
				throw new ArgumentOutOfRangeException(nameof(flatIndex));

			if (flatIndex < 0 || flatIndex >= FlatCount)
				return null;

			if (flatIndex == FlatIndex)
				return CurrentNode;

			if (flatIndex > FlatIndex)
			{
				while (flatIndex > FlatIndex)
				{
					if (MoveNext() == false)
						return null;
				}
			}
			else
			{
				while (flatIndex < FlatIndex)
				{
					if (MovePrev() == false)
						return null;
				}
			}

			Debug.Assert(FlatIndex == flatIndex);

			return CurrentNode;
		}

		public void Reset()
		{
			FlatIndex = -1;
			FlatCount = -1;

			Version++;
		}

		private struct Data
		{
			public TNode Node { get; set; }

			public int Index { get; set; }

			public override string ToString()
			{
				return Node?.ToString() ?? "Empty";
			}
		}
	}
}