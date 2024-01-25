// <copyright file="TreeNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Trees
{
	internal class TreeNode<T> : IDisposable
	{
		public TreeNode()
		{
			Children = new TreeNodeCollection<T>(this);
		}

		public TreeNode(T value)
		{
			Value = value;
			Children = new TreeNodeCollection<T>(this);
		}

		public TreeNodeCollection<T> Children { get; }

		public TreeNode<T> Parent { get; internal set; }

		public T Value { get; set; }

		public virtual void Dispose()
		{
			Value = default;

			if (Parent != null)
			{
				Parent.Children.Remove(this);
				Parent = null;
			}

			Children.DisposeChildren();
		}
	}
}