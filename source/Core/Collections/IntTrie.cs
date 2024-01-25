// <copyright file="IntTrie.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Collections
{
	internal sealed class IntTrie<TValue>
	{
		private readonly InternalNode _rootNode = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TrieNode GetNodeOrCreate(int key)
		{
			return _rootNode.GetNext(key, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TrieNode GetNodeOrCreate(ReadOnlySpan<int> span)
		{
			return _rootNode.GetNext(span, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TrieNode GetNode(int key)
		{
			return _rootNode.GetNext(key, false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TrieNode GetNode(ReadOnlySpan<int> span)
		{
			return _rootNode.GetNext(span, false);
		}

		public TrieNode RootNode
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return new TrieNode(_rootNode); }
		}

		internal sealed class InternalNode
		{
			public readonly Dictionary<int, InternalNode> Nodes = new();
			public TValue Value;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TrieNode GetNext(int key, bool create)
			{
				if (Nodes.TryGetValue(key, out var node))
					return new TrieNode(node);

				return create ? new TrieNode(Nodes[key] = new InternalNode()) : TrieNode.Empty;
			}

			public TrieNode GetNext(ReadOnlySpan<int> span, bool create)
			{
				var node = this;

				foreach (var key in span)
				{
					if (node.Nodes.TryGetValue(key, out var nextNode) == false)
					{
						if (create == false)
							return TrieNode.Empty;

						node.Nodes[key] = nextNode = new InternalNode();
					}

					node = nextNode;
				}

				return new TrieNode(node);
			}

			public TrieNode GetNext(ReadOnlySpan<int> span, int keyMask, bool create)
			{
				var node = this;

				foreach (var key in span)
				{
					var mkey = key & keyMask;

					if (node.Nodes.TryGetValue(mkey, out var nextNode) == false)
					{
						if (create == false)
							return TrieNode.Empty;

						node.Nodes[mkey] = nextNode = new InternalNode();
					}

					node = nextNode;
				}

				return new TrieNode(node);
			}
		}

		public readonly ref struct TrieNode
		{
			private readonly InternalNode _node;

			public bool IsEmpty => _node == null;

			public static TrieNode Empty => new(null);

			internal TrieNode(InternalNode node)
			{
				_node = node;
			}

			public TValue Value
			{
				get => _node == null ? default : _node.Value;
				set
				{
					if (_node == null)
						throw new InvalidOperationException("Node is empty");

					_node.Value = value;
				}
			}

			public TrieNode GetNext(int key)
			{
				return IsEmpty ? Empty : _node.GetNext(key, false);
			}

			public TrieNode GetNextOrCreate(int key)
			{
				return IsEmpty ? Empty : _node.GetNext(key, true);
			}

			public TrieNode GetNext(ReadOnlySpan<int> span)
			{
				return IsEmpty ? Empty : _node.GetNext(span, false);
			}

			public TrieNode GetNextOrCreate(ReadOnlySpan<int> span)
			{
				return IsEmpty ? Empty : _node.GetNext(span, true);
			}

			public TrieNode GetNextOrCreate(ReadOnlySpan<int> span, int mask)
			{
				return IsEmpty ? Empty : _node.GetNext(span, mask, true);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TrieNode GetNextSafe(int key)
			{
				return _node.GetNext(key, false);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TrieNode GetNextOrCreateSafe(int key)
			{
				return _node.GetNext(key, true);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TrieNode GetNextSafe(ReadOnlySpan<int> span)
			{
				return _node.GetNext(span, false);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TrieNode GetNextOrCreateSafe(ReadOnlySpan<int> span)
			{
				return _node.GetNext(span, true);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TrieNode GetNextOrCreateSafe(ReadOnlySpan<int> span, int mask)
			{
				return _node.GetNext(span, mask, true);
			}
		}
	}
}