// <copyright file="ReverseTreeEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal sealed class ReverseTreeEnumerator<T> : TreeEnumerator<T>
	{
		public ReverseTreeEnumerator(T root, ITreeEnumeratorAdvisor<T> treeAdvisor) : base(root, treeAdvisor)
		{
		}

		public ReverseTreeEnumerator(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor) : base(treeItems, treeAdvisor)
		{
		}

		public ReverseTreeEnumerator(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor) : base(treeItemsEnumerator, treeAdvisor)
		{
		}

		internal override bool AncestorsIncludesSelf => false;

		protected override bool MoveNextCore()
		{
			if (Stack == null)
			{
				if (ProceedNextRootItem() == false)
					return false;

				if (HasCurrentNode)
					return true;
			}

			while (true)
			{
				while (Stack.Count > 0)
				{
					if (HasCurrentNode)
					{
						HasCurrentNode = false;

						if (Stack.MoveNextPeakNode(out var peek))
							continue;

						HasCurrentNode = true;
						CurrentNode = Stack.Pop().Node;

						return true;
					}

					var currentNode = Stack.Peek();
					var nextNode = NodeEnumerator.Create(currentNode.CurrentChild, TreeAdvisor);

					if (nextNode.MoveNext() == false)
					{
						HasCurrentNode = true;
						CurrentNode = nextNode.Node;

						return true;
					}

					Stack.Push(nextNode);
				}

				if (ProceedNextRootItem() == false)
					return false;

				if (HasCurrentNode)
					return true;
			}
		}
	}
}