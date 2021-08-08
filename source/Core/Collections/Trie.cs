// <copyright file="Trie.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal sealed class Trie<TValue>
	{
		private const byte Mask = 0xF;

		private readonly TrieNodeInner _root = new TrieNodeInner();

		public TrieNode Root => new TrieNode(_root);

		public TrieNode GetNodeOrCreate(ReadOnlySpan<char> span)
		{
			var currentNode = _root;

			foreach (var c in span)
			{
				var local = c;
				var i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				local >>= 4;
				i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				local >>= 4;
				i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				local >>= 4;
				i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());
			}

			return new TrieNode(currentNode);
		}

		public class TrieNodeInner
		{
			public readonly TrieNodeInner[] InnerNodes = new TrieNodeInner[16];
			public TValue Value;

			public TrieNodeInner Next(char c)
			{
				var local = c;
				var i = local & Mask;
				var currentNode = this;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				local >>= 4;
				i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				local >>= 4;
				i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				local >>= 4;
				i = local & Mask;

				currentNode = currentNode.InnerNodes[i] ?? (currentNode.InnerNodes[i] = new TrieNodeInner());

				return currentNode;
			}
		}

		public readonly ref struct TrieNode
		{
			private readonly TrieNodeInner _node;

			public TrieNode(TrieNodeInner node)
			{
				_node = node;
			}

			public TValue Value
			{
				get => _node.Value;
				set => _node.Value = value;
			}

			public TrieNode Next(char c)
			{
				return new TrieNode(_node.Next(c));
			}
		}
	}
}