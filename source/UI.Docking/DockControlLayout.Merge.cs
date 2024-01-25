// <copyright file="DockControlLayout.Merge.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Collections;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Controls.Docking
{
	public sealed partial class DockControlLayout
	{
		internal void Merge()
		{
			var mergeDictionary = new Dictionary<string, DockItemLayout>();
			var groupItemsMap = new MultiMap<DockItemGroupLayout, DockItemLayout>();
			var flatItems = TreeEnumerator.GetEnumerable(Items, TreeEnumeratorAdvisor).ToList();

			foreach (var dockItemLayout in flatItems)
			{
				if (mergeDictionary.TryGetValue(dockItemLayout.ItemNameInternal, out var mergeItem) == false)
					mergeDictionary.Add(dockItemLayout.ItemNameInternal, dockItemLayout);
				else
					mergeItem.MergeProperties(dockItemLayout);

				if (dockItemLayout is not DockItemGroupLayout group)
					continue;

				groupItemsMap.Add(group, group.Items.ToList());
				group.Items.Clear();
			}

			DockItemLayout MergeItemStructure(DockItemLayout itemLayout)
			{
				var mergedItem = mergeDictionary[itemLayout.ItemNameInternal];

				if (itemLayout is not DockItemGroupLayout groupItem)
					return mergedItem;

				if (mergedItem is not DockItemGroupLayout mergedItemGroup)
					return mergedItem;

				foreach (var child in groupItemsMap[groupItem])
				{
					var mergedChild = mergeDictionary[child.ItemNameInternal];

					if (mergedChild.HasLocalValue(DockItemLayout.DockStateProperty) == false && mergedItemGroup.HasLocalValue(DockItemLayout.DockStateProperty))
						mergedChild.MergeProperty(mergedItemGroup, DockItemLayout.DockStateProperty);

					mergedItemGroup.Items.Add(MergeItemStructure(child));
				}

				return mergedItemGroup;
			}

			var itemsCopy = Items.ToList();

			Items.Clear();

			foreach (var itemLayout in itemsCopy)
				Items.Add(MergeItemStructure(itemLayout));
		}
	}
}