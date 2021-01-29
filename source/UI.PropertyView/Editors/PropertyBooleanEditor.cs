// <copyright file="PropertyBooleanEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyBooleanEditor : PropertyDropDownListViewEditor<bool>
	{
		private static readonly PropertyListViewItemSource<bool> FalseValueItem = new(false, "False");
		private static readonly PropertyListViewItemSource<bool> TrueValueItem = new(true, "True");

		private static readonly ReadOnlyCollection<PropertyListViewItemSource<bool>> StaticItems = new(new List<PropertyListViewItemSource<bool>>
		{
			TrueValueItem, FalseValueItem
		});

		protected override IReadOnlyCollection<PropertyListViewItemSource<bool>> Items => StaticItems;

		protected override PropertyListViewItemSource<bool> GetItemByValue(bool value)
		{
			return value ? TrueValueItem : FalseValueItem;
		}
	}
}