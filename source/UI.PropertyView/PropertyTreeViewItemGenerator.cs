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
		private readonly Stack<PropertyViewCategory> _categories = new Stack<PropertyViewCategory>();
		private readonly Stack<PropertyViewItem> _items = new Stack<PropertyViewItem>();

		internal PropertyViewItemGenerator(PropertyViewControl propertyView)
		{
			PropertyView = propertyView;
		}

		public PropertyViewControl PropertyView { get; }

		protected override void AttachItem(TreeViewItem item, object itemSource)
		{
			switch (item, itemSource)
			{
				case (PropertyViewItem viewItem, PropertyItem dataItem):

					viewItem.PropertyItem = dataItem;

					return;

				case (PropertyViewCategory viewItem, PropertyCategory dataItem):

					viewItem.Category = dataItem;

					return;
			}

			throw new InvalidOperationException();
		}

		protected override TreeViewItem CreateItem(object itemSource)
		{
			return itemSource switch
			{
				PropertyCategory _ => _categories.Count > 0 ? _categories.Pop() : new PropertyViewCategory(PropertyView),
				PropertyItem _ => _items.Count > 0 ? _items.Pop() : new PropertyViewItem(PropertyView),
				_ => throw new InvalidOperationException()
			};
		}

		protected override void DetachItem(TreeViewItem item, object itemSource)
		{
			switch (item, itemSource)
			{
				case (PropertyViewItem viewItem, PropertyItem _):

					viewItem.PropertyItem = null;

					return;

				case (PropertyViewCategory viewItem, PropertyCategory _):

					viewItem.Category = null;

					return;
			}

			throw new InvalidOperationException();
		}

		protected override void DisposeItem(TreeViewItem item, object itemSource)
		{
			switch (item)
			{
				case PropertyViewCategory propertyViewCategory:
					_categories.Push(propertyViewCategory);
					break;
				case PropertyViewItem propertyViewItem:
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