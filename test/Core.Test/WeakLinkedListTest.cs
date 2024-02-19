// <copyright file="WeakLinkedListTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.Core.Test
{
	[TestFixture]
	public class WeakLinkedListTest
	{
		#region  Methods

		[Test(Description = "Init")]
		public void InitTest()
		{
			InitTestImpl(0);
			InitTestImpl(1);
			InitTestImpl(2);
			InitTestImpl(10);
		}

		private void InitTestImpl(int count)
		{
			List<int> values;
			List<ValueNode> nodes;
			WeakLinkedList<ValueNode> list;

			InitWeakLinkedList(count, out values, out nodes, out list);
			Assert.True(list.SequenceEqual(nodes), "Collections should be the same");
		}

		private void InitWeakLinkedList(int count, out List<int> values, out List<ValueNode> nodes, out WeakLinkedList<ValueNode> list)
		{
			values = Enumerable.Range(0, count).ToList();
			nodes = values.Select(v => new ValueNode(v)).ToList();

			list = new WeakLinkedList<ValueNode>();
			foreach (var valueNode in nodes)
				list.Add(valueNode);
		}

		[Test(Description = "Remove")]
		public void RemoveTest()
		{
			RemoveTestImpl(1, 0);

			RemoveTestImpl(2, 0, 1);
			RemoveTestImpl(2, 1, 0);

			RemoveTestImpl(3, 0);
			RemoveTestImpl(3, 1);
			RemoveTestImpl(3, 2);

			RemoveTestImpl(3, 0, 1);
			RemoveTestImpl(3, 0, 2);
			RemoveTestImpl(3, 1, 0);
			RemoveTestImpl(3, 1, 2);
			RemoveTestImpl(3, 2, 0);
			RemoveTestImpl(3, 2, 1);

			RemoveTestImpl(3, 0, 1, 2);
			RemoveTestImpl(3, 0, 2, 1);
			RemoveTestImpl(3, 1, 0, 2);
			RemoveTestImpl(3, 1, 2, 0);
			RemoveTestImpl(3, 2, 0, 1);
			RemoveTestImpl(3, 2, 1, 0);

			RemoveTestImpl(10, 1, 2, 3);
			RemoveTestImpl(5, 4, 1, 2, 3, 0);
			RemoveTestImpl(10, 4, 1, 2, 3, 0);
		}

		private void RemoveTestImpl(int count, params int[] indices)
		{
			List<int> values;
			List<ValueNode> nodes;
			WeakLinkedList<ValueNode> list;
			InitWeakLinkedList(count, out values, out nodes, out list);

			foreach (var node in indices.Select(i => nodes[i]).ToList())
			{
				nodes.Remove(node);
				list.Remove(node);
			}

			Assert.True(list.SequenceEqual(nodes), "Collections should be the same");
		}

		#endregion
	}
}