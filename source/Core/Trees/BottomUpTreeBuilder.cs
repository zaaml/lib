// <copyright file="BottomUpTreeNodeBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal static class BottomUpTreeBuilder<T>
	{
		public static TreeNode<T> Build(IEnumerable<T> items, int degree, Func<TreeNode<T>, T> valueFactory)
		{
			var nodePool = TreeNodePool<T>.Shared;
			TreeNode<T> leafStart = null;
			{
				TreeNode<T> prev = null;

				foreach (var item in items)
				{
					var node = nodePool.Rent();

					node.Value = item;
					leafStart ??= node;

					if (prev != null)
						prev.Parent = node;

					prev = node;
				}
			}

			while (true)
			{
				TreeNode<T> prev = null;
				TreeNode<T> start = null;
				TreeNode<T> current = null;

				while (leafStart != null)
				{
					current ??= nodePool.Rent();

					var currentLeaf = leafStart;

					leafStart = leafStart.Parent;

					currentLeaf.Parent = null;
					current.Children.Add(currentLeaf);

					if (current.Children.Count < degree)
						continue;

					current.Value = valueFactory(current);

					start ??= current;

					if (prev != null)
						prev.Parent = current;

					prev = current;
					current = null;
				}

				if (current != null)
				{
					current.Value = valueFactory(current);

					if (prev != null)
						prev.Parent = current;
				}

				start ??= current;

				if (start?.Parent == null)
					return start;

				leafStart = start;
			}
		}
	}
}