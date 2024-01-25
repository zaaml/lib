// <copyright file="PropertyFontStretchEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyFontStretchEditor : PropertyDropDownListViewEditor<FontStretch>
	{
		private static readonly ReadOnlyCollection<PropertyListViewItemSource<FontStretch>> StaticItems;
		private static readonly Dictionary<FontStretch, PropertyListViewItemSource<FontStretch>> ItemDictionary;

		static PropertyFontStretchEditor()
		{
			StaticItems = new ReadOnlyCollection<PropertyListViewItemSource<FontStretch>>(new List<PropertyListViewItemSource<FontStretch>>
			{
				new(FontStretches.UltraCondensed, nameof(FontStretches.UltraCondensed)),
				new(FontStretches.ExtraCondensed, nameof(FontStretches.ExtraCondensed)),
				new(FontStretches.Condensed, nameof(FontStretches.Condensed)),
				new(FontStretches.SemiCondensed, nameof(FontStretches.SemiCondensed)),
				new(FontStretches.Normal, nameof(FontStretches.Normal)),
				new(FontStretches.Medium, nameof(FontStretches.Medium)),
				new(FontStretches.SemiExpanded, nameof(FontStretches.SemiExpanded)),
				new(FontStretches.Expanded, nameof(FontStretches.Expanded)),
				new(FontStretches.ExtraExpanded, nameof(FontStretches.ExtraExpanded)),
				new(FontStretches.UltraExpanded, nameof(FontStretches.UltraExpanded))
			});

			ItemDictionary = new Dictionary<FontStretch, PropertyListViewItemSource<FontStretch>>();

			foreach (var item in StaticItems)
			{
				if (ItemDictionary.ContainsKey(item.Value) == false)
					ItemDictionary.Add(item.Value, item);
			}
		}

		protected override IReadOnlyCollection<PropertyListViewItemSource<FontStretch>> Items => StaticItems;

		protected override PropertyListViewItemSource<FontStretch> GetItemByValue(FontStretch value)
		{
			return ItemDictionary.TryGetValue(value, out var item) ? item : null;
		}
	}
}