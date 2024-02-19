// <copyright file="DataTree.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	internal class DataTree
	{
		private readonly Dictionary<string, DataTreeItem> _itemsDictionary = new();

		public DataTree(int levelCount, int levelSize, bool isExpanded)
		{
			for (var i = 0; i < levelSize; i++)
			{
				var dataTreeItem = CreateItem(i, null, isExpanded);

				Items.Add(dataTreeItem);

				Build(dataTreeItem, levelCount, levelSize, isExpanded);
			}
		}

		public List<DataTreeItem> Items { get; } = new();

		private void Build(DataTreeItem item, int levelCount, int levelSize, bool isExpanded)
		{
			if (item.Level >= levelCount)
				return;

			for (var i = 0; i < levelSize; i++)
			{
				var dataTreeItem = CreateItem(i, item, isExpanded);

				item.Children.Add(dataTreeItem);

				Build(dataTreeItem, levelCount, levelSize, isExpanded);
			}
		}

		public void Collapse(params string[] keys)
		{
			foreach (var key in keys)
				GetItem(key).IsExpanded = false;
		}

		private DataTreeItem CreateItem(int index, DataTreeItem parent, bool isExpanded)
		{
			var globalIndex = _itemsDictionary.Count;
			var dataTreeItem = new DataTreeItem(this, parent, index, globalIndex)
			{
				IsExpanded = isExpanded
			};

			var key = CreateKey(dataTreeItem);

			dataTreeItem.Caption = key;

			_itemsDictionary[key] = dataTreeItem;

			return dataTreeItem;
		}

		private string CreateKey(DataTreeItem item)
		{
			var current = item;
			var strIndex = new StringBuilder();

			while (current != null)
			{
				strIndex.Insert(0, current.LocalIndex + 1);

				current = current.Parent;
			}

			return $"Item_{strIndex}";
		}

		public void Expand(params string[] keys)
		{
			foreach (var key in keys)
				GetItem(key).IsExpanded = true;
		}

		public DataTreeItem GetItem(string key)
		{
			return _itemsDictionary[key];
		}
	}
}