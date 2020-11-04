// <copyright file="PropertyBooleanEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyBooleanEditor : PropertyDropDownListViewEditor<bool>
	{
		private readonly PropertyListViewItemSource<bool> _falseValueItem = new PropertyListViewItemSource<bool>(false, "False");
		private readonly PropertyListViewItemSource<bool> _trueValueItem = new PropertyListViewItemSource<bool>(true, "True");

		public PropertyBooleanEditor()
		{
			Items = new ReadOnlyCollection<PropertyListViewItemSource<bool>>(new List<PropertyListViewItemSource<bool>>
			{
				_trueValueItem, _falseValueItem
			});
		}

		protected override IReadOnlyCollection<PropertyListViewItemSource<bool>> Items { get; }

		protected override PropertyListViewItemSource<bool> GetItemByValue(bool value)
		{
			return value ? _trueValueItem : _falseValueItem;
		}
	}
}