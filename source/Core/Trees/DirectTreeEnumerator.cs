// <copyright file="DirectTreeEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal sealed class DirectTreeEnumerator<T> : TreeEnumerator<T>
	{
		public DirectTreeEnumerator(T root, ITreeEnumeratorAdvisor<T> treeAdvisor) : base(root, treeAdvisor)
		{
		}

		public DirectTreeEnumerator(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor) : base(treeItems, treeAdvisor)
		{
		}

		public DirectTreeEnumerator(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor) : base(treeItemsEnumerator, treeAdvisor)
		{
		}

		internal override bool AncestorsIncludesSelf => true;

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
					var tmp = Stack.Peek();

					if (tmp.IsVisited == false)
					{
						CurrentNode = Stack.Peek().Node;
						Stack.VisitPeakNode();
						HasCurrentNode = true;

						return true;
					}

					if (tmp.HasCurrent == false)
						Stack.MoveNextPeakNode(out tmp);

					if (tmp.HasNext == false && tmp.HasCurrent == false)
					{
						while (Stack.Count > 0 && Stack.MoveNextPeakNode(out tmp) == false)
							Stack.Pop();

						if (Stack.Count == 0)
							break;
					}

					Stack.Push(NodeEnumerator.Create(tmp.CurrentChild, TreeAdvisor));
				}

				if (ProceedNextRootItem() == false)
					return false;

				if (HasCurrentNode)
					return true;
			}
		}
	}
}