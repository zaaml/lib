// <copyright file="TreeNodePool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace Zaaml.Core.Trees
{
	internal sealed class TreeNodePool<T>
	{
		private static readonly ThreadLocal<TreeNodePool<T>> ThreadLocalInstance = new(() => new TreeNodePool<T>());

		private TreeNode<T> Root { get; set; }

		public static TreeNodePool<T> Shared => ThreadLocalInstance.Value;

		public TreeNode<T> Rent()
		{
			if (Root == null)
				return new PooledTreeNode(this);

			var node = Root;

			Root = Root.Parent;
			node.Parent = null;

			return node;
		}

		public void Return(TreeNode<T> node)
		{
			if (node is not PooledTreeNode pooledNode || ReferenceEquals(pooledNode.Pool, this) == false)
				throw new InvalidOperationException();

			node.Parent = Root;

			Root = node;
		}

		private sealed class PooledTreeNode : TreeNode<T>
		{
			public PooledTreeNode(TreeNodePool<T> pool)
			{
				Pool = pool;
			}

			public TreeNodePool<T> Pool { get; }

			public override void Dispose()
			{
				base.Dispose();

				Pool.Return(this);
			}
		}
	}
}