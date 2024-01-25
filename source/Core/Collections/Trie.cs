// <copyright file="Trie.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal sealed class Trie<TKey, TValue>
	{
		private readonly InternalNode _rootNode = new();

		public TrieNode GetNodeOrCreate(TKey key)
		{
			return _rootNode.GetNext(key, true);
		}

		public TrieNode GetNodeOrCreate(ReadOnlySpan<TKey> span)
		{
			return _rootNode.GetNext(span, true);
		}

		public TrieNode GetNode(TKey key)
		{
			return _rootNode.GetNext(key, false);
		}

		public TrieNode GetNode(ReadOnlySpan<TKey> span)
		{
			return _rootNode.GetNext(span, false);
		}

		public TrieNode RootNode => new(_rootNode);

		internal sealed class InternalNode
		{
			public readonly Dictionary<TKey, InternalNode> Nodes = new();
			public TValue Value;

			public TrieNode GetNext(TKey key, bool create)
			{
				if (Nodes.TryGetValue(key, out var node))
					return new TrieNode(node);

				return create ? new TrieNode(Nodes[key] = new InternalNode()) : TrieNode.Empty;
			}

			public TrieNode GetNext(ReadOnlySpan<TKey> span, bool create)
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

			public TrieNode GetNext(TKey key)
			{
				return IsEmpty ? Empty : _node.GetNext(key, false);
			}

			public TrieNode GetNextOrCreate(TKey key)
			{
				return IsEmpty ? Empty : _node.GetNext(key, true);
			}

			public TrieNode GetNext(ReadOnlySpan<TKey> span)
			{
				return IsEmpty ? Empty : _node.GetNext(span, false);
			}

			public TrieNode GetNextOrCreate(ReadOnlySpan<TKey> span)
			{
				return IsEmpty ? Empty : _node.GetNext(span, true);
			}
		}
	}
}