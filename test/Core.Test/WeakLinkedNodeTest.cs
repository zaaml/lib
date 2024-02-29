// <copyright file="WeakLinkedNodeTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.Core.Test
{
	[TestFixture]
	public class WeakLinkedNodeTest
	{
		[Test]
		public void CleanTest()
		{
			InitWeakLinkedList(20, out var nodes, out var weakNodes);

			GC.GetTotalMemory(true);

			nodes[6] = null;
			nodes[7] = null;
			nodes[9] = null;
			nodes[10] = null;
			nodes[12] = null;
			nodes[14] = null;

			// Remove 3 last
			nodes[17] = null;
			nodes[18] = null;
			nodes[19] = null;

			// Remove 3 first
			nodes[0] = null;
			nodes[1] = null;
			nodes[2] = null;

			// Collect garbage
			GC.GetTotalMemory(true);

			// Clean up linked list
			var head = weakNodes.First();

			WeakLinkedNode.Clean(ref head, out var tail);

			// New head
			Assert.AreSame(weakNodes.First(w => w.IsAlive), head);

			// New tail should point on item8
			Assert.AreSame(weakNodes.Last(w => w.IsAlive), tail);

			VerifySameCollections(nodes, weakNodes);
		}

		[Test]
		public void InitListTest()
		{
			InitListTestImpl(1);
			InitListTestImpl(2);
			InitListTestImpl(10);
		}

		private static void InitListTestImpl(int count)
		{
			InitWeakLinkedList(count, out var nodes, out var weakNodes);

			GC.GetTotalMemory(true);

			VerifySameCollections(nodes, weakNodes);

			var head = weakNodes.First();

			WeakLinkedNode.Clean(ref head, out _);

			VerifySameCollections(nodes, weakNodes);
		}

		private static void InitWeakLinkedList(int count, out List<ValueNode> nodes, out List<WeakLinkedNode<ValueNode>> weakNodes)
		{
			nodes = Enumerable.Range(0, count).Select(v => new ValueNode(v)).ToList();
			weakNodes = nodes.Select(WeakLinkedNode.Create).ToList();

			// Init linked list
			var head = weakNodes[0];

			for (var i = 1; i < weakNodes.Count; i++)
			{
				head.Next = weakNodes[i];
				head = weakNodes[i];
			}
		}

		private static void VerifySameCollections(List<ValueNode> nodes, List<WeakLinkedNode<ValueNode>> weakNodes)
		{
			var head = weakNodes.First(x => x.IsAlive);

			// Get weak values
			var weakValues = head.EnumerateAlive(false).Select(v => v.Value).ToList();

			// Get cleaned values
			var cleanedValues = nodes.Where(x => x != null).Select(x => x.Value).ToList();

			// Weak values should equals cleaned values
			Assert.True(cleanedValues.SequenceEqual(weakValues), "Collections should be the same");
		}
	}
}