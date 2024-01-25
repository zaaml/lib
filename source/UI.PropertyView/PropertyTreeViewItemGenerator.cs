// <copyright file="PropertyTreeViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemGenerator : TreeViewItemGeneratorBase
	{
		private readonly Stack<PropertyGridViewCategory> _categories = new();
		private readonly Stack<PropertyGridViewItem> _items = new();

		internal PropertyViewItemGenerator()
		{
		}

		public PropertyViewControl PropertyViewControl { get; set; }

		protected override void AttachItem(TreeViewItem item, object source)
		{
			switch (item, source)
			{
				case (PropertyGridViewItem viewItem, PropertyItem dataItem):

					viewItem.PropertyItem = dataItem;

					return;

				case (PropertyGridViewCategory viewItem, PropertyCategory dataItem):

					viewItem.Category = dataItem;

					return;
			}

			throw new InvalidOperationException();
		}

		protected override TreeViewItem CreateItem(object source)
		{
			return source switch
			{
				PropertyCategory _ => _categories.Count > 0 ? _categories.Pop() : new PropertyGridViewCategory(PropertyViewControl),
				PropertyItem _ => _items.Count > 0 ? _items.Pop() : new PropertyGridViewItem(PropertyViewControl),
				_ => throw new InvalidOperationException()
			};
		}

		protected override void DetachItem(TreeViewItem item, object source)
		{
			switch (item, source)
			{
				case (PropertyGridViewItem viewItem, PropertyItem _):

					viewItem.PropertyItem = null;

					return;

				case (PropertyGridViewCategory viewItem, PropertyCategory _):

					viewItem.Category = null;

					return;
			}

			throw new InvalidOperationException();
		}

		protected override void DisposeItem(TreeViewItem item, object source)
		{
			switch (item)
			{
				case PropertyGridViewCategory propertyViewCategory:
					_categories.Push(propertyViewCategory);
					break;
				case PropertyGridViewItem propertyViewItem:
					_items.Push(propertyViewItem);
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		protected override IEnumerable GetTreeNodes(object treeNodeData)
		{
			return treeNodeData switch
			{
				PropertyCategory propertyItemCategory => propertyItemCategory.PropertyItems,
				PropertyItem propertyItem => propertyItem.ChildPropertyItems,
				_ => throw new InvalidOperationException()
			};
		}

		protected override bool IsExpanded(object treeNodeData)
		{
			return treeNodeData is PropertyCategory;
		}
	}
}