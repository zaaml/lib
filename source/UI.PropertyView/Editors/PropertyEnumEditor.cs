// <copyright file="PropertyEnumEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zaaml.Core.Converters;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyEnumEditor<TEnum> : PropertyDropDownListViewEditor<TEnum> where TEnum : struct, Enum
	{
		private static readonly Type EnumType = typeof(TEnum);

		public PropertyEnumEditor()
		{
			var propertyDropDownListViewItems = new List<PropertyListViewItemSource<TEnum>>();

			foreach (TEnum value in Enum.GetValues(EnumType))
				propertyDropDownListViewItems.Add(new PropertyListViewItemSource<TEnum>(value, Enum.GetName(EnumType, value)));

			Items = new ReadOnlyCollection<PropertyListViewItemSource<TEnum>>(propertyDropDownListViewItems);
		}

		protected override IReadOnlyCollection<PropertyListViewItemSource<TEnum>> Items { get; }

		protected override PropertyListViewItemSource<TEnum> GetItemByValue(TEnum value)
		{
			var longValue = EnumConverter<TEnum>.Convert(value);

			foreach (var propertyDropDownListViewItem in Items)
			{
				if (EnumConverter<TEnum>.Convert(propertyDropDownListViewItem.Value) == longValue)
					return propertyDropDownListViewItem;
			}

			return null;
		}
	}
}