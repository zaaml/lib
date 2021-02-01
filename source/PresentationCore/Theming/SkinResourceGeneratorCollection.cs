// <copyright file="SkinResourceGeneratorCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class SkinResourceGeneratorCollection : Collection<SkinResourceGenerator>
	{
		internal SkinResourceGeneratorCollection(SkinDictionary skinDictionary)
		{
			SkinDictionary = skinDictionary;
		}

		public SkinDictionary SkinDictionary { get; }

		protected override void ClearItems()
		{
			foreach (var generator in this)
				generator.SkinDictionary = null;

			base.ClearItems();
		}

		protected override void InsertItem(int index, SkinResourceGenerator item)
		{
			base.InsertItem(index, item);

			item.SkinDictionary = SkinDictionary;
		}

		protected override void RemoveItem(int index)
		{
			var generator = this[index];

			generator.SkinDictionary = null;

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, SkinResourceGenerator item)
		{
			var generator = this[index];

			generator.SkinDictionary = null;

			base.SetItem(index, item);

			generator.SkinDictionary = SkinDictionary;
		}
	}
}