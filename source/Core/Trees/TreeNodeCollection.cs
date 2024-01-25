// <copyright file="TreeNodeCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal class TreeNodeCollection<T> : IReadOnlyList<TreeNode<T>>
	{
		public TreeNodeCollection(TreeNode<T> owner)
		{
			Owner = owner;
		}

		private List<TreeNode<T>> List { get; } = new();

		private TreeNode<T> Owner { get; }

		public void Add(TreeNode<T> node)
		{
			if (ReferenceEquals(node.Parent, null) == false)
				throw new InvalidOperationException();

			List.Add(node);

			node.Parent = Owner;
		}

		public void Remove(TreeNode<T> node)
		{
			if (ReferenceEquals(node.Parent, Owner) == false)
				throw new InvalidOperationException();

			node.Parent = null;

			List.Remove(node);
		}

		public IEnumerator<TreeNode<T>> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) List).GetEnumerator();
		}

		public int Count => List.Count;

		public TreeNode<T> this[int index] => List[index];

		public void Clear()
		{
			foreach (var treeNode in List) 
				treeNode.Parent = null;

			List.Clear();
		}

		internal void DisposeChildren()
		{
			foreach (var treeNode in List)
			{
				treeNode.Parent = null;
				treeNode.Dispose();
			}

			List.Clear();
		}
	}
}