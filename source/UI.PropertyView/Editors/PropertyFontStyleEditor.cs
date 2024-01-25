// <copyright file="PropertyFontStyleEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyFontStyleEditor : PropertyDropDownListViewEditor<FontStyle>
	{
		private static readonly ReadOnlyCollection<PropertyListViewItemSource<FontStyle>> StaticItems;
		private static readonly Dictionary<FontStyle, PropertyListViewItemSource<FontStyle>> ItemDictionary;

		static PropertyFontStyleEditor()
		{
			StaticItems = new ReadOnlyCollection<PropertyListViewItemSource<FontStyle>>(new List<PropertyListViewItemSource<FontStyle>>
			{
				new(FontStyles.Normal, nameof(FontStyles.Normal)),
				new(FontStyles.Italic, nameof(FontStyles.Italic)),
				new(FontStyles.Oblique, nameof(FontStyles.Oblique)),
			});

			ItemDictionary = new Dictionary<FontStyle, PropertyListViewItemSource<FontStyle>>();

			foreach (var item in StaticItems)
			{
				if (ItemDictionary.ContainsKey(item.Value) == false)
					ItemDictionary.Add(item.Value, item);
			}
		}

		protected override IReadOnlyCollection<PropertyListViewItemSource<FontStyle>> Items => StaticItems;

		protected override PropertyListViewItemSource<FontStyle> GetItemByValue(FontStyle value)
		{
			return ItemDictionary.TryGetValue(value, out var item) ? item : null;
		}
	}
}