// <copyright file="TreeEnumeratorTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Zaaml.Core.Trees;

namespace Zaaml.Core.Test.Trees
{
	internal sealed class TreeEnumeratorTest
	{
		private bool CheckEnumerator(ITreeEnumerator<TreeNode> treeEnumerator, IEnumerable<int> expectedValues)
		{
			using (var expectedEnumerator = expectedValues.GetEnumerator())
			{
				while (expectedEnumerator.MoveNext())
				{
					if (treeEnumerator.MoveNext() == false)
						throw new Exception("Move next");

					if (Equals(expectedEnumerator.Current, treeEnumerator.Current.Value) == false)
						throw new Exception("Current");
				}
			}

			if (treeEnumerator.MoveNext())
				throw new Exception("Finished");

			return true;
		}

		[Test(Description = "EnumeratorTest")]
		public void EnumeratorTest()
		{
			var tree = GetTestTree();

			// Direct from root
			Assert.True(CheckEnumerator(TreeEnumerator.GetEnumerator(tree, TreeNodeEnumeratorAdvisor.Instance), [1, 11, 12, 13, 131, 132]));

			// Direct from collection
			Assert.True(CheckEnumerator(TreeEnumerator.GetEnumerator(tree.Children, TreeNodeEnumeratorAdvisor.Instance), [11, 12, 13, 131, 132]));

			// Reverse from root
			Assert.True(CheckEnumerator(TreeEnumerator.GetReverseEnumerator(tree, TreeNodeEnumeratorAdvisor.Instance), [11, 12, 131, 132, 13, 1]));

			// Reverse from collection
			Assert.True(CheckEnumerator(TreeEnumerator.GetReverseEnumerator(tree.Children, TreeNodeEnumeratorAdvisor.Instance), [11, 12, 131, 132, 13]));

			using var enumerator = TreeEnumerator.GetEnumerator(tree, TreeNodeEnumeratorAdvisor.Instance);

			enumerator.MoveNext();

			// Current = 1
			Assert.AreEqual(true, enumerator.GetAncestorsEnumerator().Enumerate().Select(n => n.Value).SequenceEqual([]));

			enumerator.MoveNext();

			// Current = 11
			Assert.AreEqual(true, enumerator.GetAncestorsEnumerator().Enumerate().Select(n => n.Value).SequenceEqual([1]));

			enumerator.MoveNext();

			// Current = 12
			Assert.AreEqual(true, enumerator.GetAncestorsEnumerator().Enumerate().Select(n => n.Value).SequenceEqual([1]));

			enumerator.MoveNext();

			// Current = 13
			Assert.AreEqual(true, enumerator.GetAncestorsEnumerator().Enumerate().Select(n => n.Value).SequenceEqual([1]));

			enumerator.MoveNext();

			// Current = 131
			Assert.AreEqual(true, enumerator.GetAncestorsEnumerator().Enumerate().Select(n => n.Value).SequenceEqual([13, 1]));

			enumerator.MoveNext();

			// Current = 132
			Assert.AreEqual(true, enumerator.GetAncestorsEnumerator().Enumerate().Select(n => n.Value).SequenceEqual([13, 1]));

			Assert.AreEqual(false, enumerator.MoveNext());
		}

		private static TreeNode GetTestTree()
		{
			return new TreeNode(1)
			{
				Children =
				{
					new TreeNode(11),
					new TreeNode(12),
					new TreeNode(13)
					{
						Children =
						{
							new TreeNode(131),
							new TreeNode(132)
						}
					}
				}
			};
		}

		private class TreeNodeEnumeratorAdvisor : ITreeEnumeratorAdvisor<TreeNode>
		{
			public static readonly ITreeEnumeratorAdvisor<TreeNode> Instance = new TreeNodeEnumeratorAdvisor();

			private TreeNodeEnumeratorAdvisor()
			{
			}

			public IEnumerator<TreeNode> GetChildren(TreeNode node)
			{
				return node.Children.GetEnumerator();
			}
		}

		private class TreeNode
		{
			public TreeNode(int value)
			{
				Value = value;
			}

			public TreeNode()
			{
			}

			public List<TreeNode> Children { get; } = new();

			public int Value { get; }
		}
	}
}