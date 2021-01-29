// <copyright file="PropertyFontWeightEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyFontWeightEditor : PropertyDropDownListViewEditor<FontWeight>
	{
		private static readonly ReadOnlyCollection<PropertyListViewItemSource<FontWeight>> StaticItems;
		private static readonly Dictionary<FontWeight, PropertyListViewItemSource<FontWeight>> ItemDictionary;

		static PropertyFontWeightEditor()
		{
			StaticItems = new ReadOnlyCollection<PropertyListViewItemSource<FontWeight>>(new List<PropertyListViewItemSource<FontWeight>>
			{
				new(FontWeights.Thin, nameof(FontWeights.Thin)),
				new(FontWeights.ExtraLight, nameof(FontWeights.ExtraLight)),
				new(FontWeights.UltraLight, nameof(FontWeights.UltraLight)),
				new(FontWeights.Light, nameof(FontWeights.Light)),
				new(FontWeights.Normal, nameof(FontWeights.Normal)),
				new(FontWeights.Regular, nameof(FontWeights.Regular)),
				new(FontWeights.Medium, nameof(FontWeights.Medium)),
				new(FontWeights.DemiBold, nameof(FontWeights.DemiBold)),
				new(FontWeights.SemiBold, nameof(FontWeights.SemiBold)),
				new(FontWeights.Bold, nameof(FontWeights.Bold)),
				new(FontWeights.ExtraBold, nameof(FontWeights.ExtraBold)),
				new(FontWeights.UltraBold, nameof(FontWeights.UltraBold)),
				new(FontWeights.Black, nameof(FontWeights.Black)),
				new(FontWeights.Heavy, nameof(FontWeights.Heavy)),
				new(FontWeights.ExtraBlack, nameof(FontWeights.ExtraBlack)),
				new(FontWeights.UltraBlack, nameof(FontWeights.UltraBlack))
			});

			ItemDictionary = new Dictionary<FontWeight, PropertyListViewItemSource<FontWeight>>();

			foreach (var item in StaticItems)
			{
				if (ItemDictionary.ContainsKey(item.Value) == false)
					ItemDictionary.Add(item.Value, item);
			}
		}

		protected override IReadOnlyCollection<PropertyListViewItemSource<FontWeight>> Items => StaticItems;

		protected override PropertyListViewItemSource<FontWeight> GetItemByValue(FontWeight value)
		{
			return ItemDictionary.TryGetValue(value, out var item) ? item : null;
		}
	}
}