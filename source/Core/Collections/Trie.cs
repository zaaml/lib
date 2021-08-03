// <copyright file="Trie.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal sealed class Trie<TValue>
	{
		#region Static Fields and Constants

		private const byte Mask = 0xF;

		#endregion

		#region Fields

		private readonly TrieNodeInner _root = new TrieNodeInner();

		#endregion

		#region  Methods

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

		#endregion

		#region  Nested Types

		public class TrieNodeInner
		{
			#region Fields

			public readonly TrieNodeInner[] InnerNodes = new TrieNodeInner[16];
			public TValue Value;

			#endregion
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
		}

		#endregion
	}
}