// <copyright file="HierarchyViewTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	[TestFixture]
	public class HierarchyViewTest
	{
		private const HierarchyView.DumpOptions DefaultDumpOptions = HierarchyView.DumpOptions.Padding | HierarchyView.DumpOptions.ExpansionGlyph | HierarchyView.DumpOptions.LineNumbers;

		private static readonly ITreeEnumeratorAdvisor<DataTreeItem> TreeEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<DataTreeItem>(ChildrenFactory);

		private static IEnumerator<DataTreeItem> ChildrenFactory(DataTreeItem arg)
		{
			return arg.Children.GetEnumerator();
		}

		[Test(Description = "CursorTest")]
		public void CursorTest()
		{
			var dataTree = new DataTree(4, 3, true);

			var tree = new TestHierarchyView(new TreeAdvisor())
			{
				Source = dataTree.Items
			};

			var enumeratorCaptions = TreeEnumerator.GetEnumerator(dataTree.Items, TreeEnumeratorAdvisor).Enumerate().Select(t => t.Caption).ToList();
			var directCaptions = new List<string>();

			for (var iNode = 0; iNode < tree.VisibleFlatCount; iNode++)
			{
				var treeNode = tree.GetNode(iNode);
				var treeItem = (DataTreeItem)treeNode.Data;

				directCaptions.Add(treeItem.Caption);
			}

			Assert.True(enumeratorCaptions.SequenceEqual(directCaptions));

			var reverseCaptions = new List<string>();

			for (var iNode = tree.VisibleFlatCount - 1; iNode >= 0; iNode--)
			{
				var treeNode = tree.GetNode(iNode);
				var treeItem = (DataTreeItem)treeNode.Data;

				reverseCaptions.Add(treeItem.Caption);
			}

			var item189 = dataTree.GetItem("Item_223");
			var index = tree.FindDataIndex(item189);

			Assert.AreEqual(index, 189);
			Assert.True(enumeratorCaptions.AsEnumerable().Reverse().SequenceEqual(reverseCaptions));
		}

		[Test(Description = "ExpandCollapseTest")]
		public void ExpandCollapseTest()
		{
			//var dataTree = new DataTree(3, 2, false);

			//var tree = new TestHierarchyView(new TreeAdvisor())
			//{
			//	Source = dataTree.Items
			//};

			//dataTree.Expand("Item_111", "Item_121", "Item_211", "Item_221");

			//var item_1 = tree.GetNode(tree.FindDataIndex(dataTree.GetItem("Item_1")));

			//item_1.IsExpanded = true;

			//var item_11 = tree.GetNode(tree.FindDataIndex(dataTree.GetItem("Item_11")));

			//item_11.IsExpanded = true;
			//tree.Verify();

			//item_1.IsExpanded = false;
			//tree.Verify();

			//item_11.IsExpanded = false;
			//tree.Verify();

			//item_1.IsExpanded = true;
			//tree.Verify();

			//tree.CollapseAll(ExpandCollapseMode.Default);
			//tree.Verify();

			//tree.ExpandAll(ExpandCollapseMode.Default);
			//tree.Verify();

			//tree.CollapseAll(ExpandCollapseMode.Default);
			//tree.Verify();

			//item_1.ExpandRecursive(ExpandCollapseMode.Default);
			//tree.Verify();

			//item_1.CollapseRecursive(ExpandCollapseMode.Default);
			//tree.Verify();
		}

		[Test(Description = "LoadAllTest")]
		public void LoadAllTest()
		{
			var dataTree = new DataTree(3, 2, false);

			var tree = new TestHierarchyView(new TreeAdvisor())
			{
				Source = dataTree.Items
			};
		}

		[Test(Description = "LoadTest")]
		public void LoadTest()
		{
			var dataTree = new DataTree(2, 2, true);

			var tree = new TestHierarchyView(new TreeAdvisor())
			{
				Source = dataTree.Items
			};

			var strTree = tree.Dump(DefaultDumpOptions);
		}
	}
}