// <copyright file="MenuItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public sealed class MenuItemGenerator : MenuItemGeneratorBase
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<MenuItemTemplate, MenuItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		private static readonly DependencyPropertyKey GeneratorPropertyKey = DPM.RegisterAttachedReadOnly<MenuItemGenerator, MenuItemGenerator>
			("Generator");

		public static readonly DependencyProperty GeneratorProperty = GeneratorPropertyKey.DependencyProperty;

		#endregion

		#region Fields

		private readonly GeneratorDataTemplateHelper<MenuItemBase, MenuItem> _generatorDataTemplateHelper = new GeneratorDataTemplateHelper<MenuItemBase, MenuItem>();

		#endregion

		#region Properties

		public MenuItemTemplate ItemTemplate
		{
			get => (MenuItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		#endregion

		#region  Methods

		protected override void AttachItem(MenuItemBase item, object itemSource)
		{
			_generatorDataTemplateHelper.AttachDataContext(item, itemSource);

			if (ItemTemplate != null)
				return;

			if (item is HeaderedMenuItem headeredMenuItem)
				headeredMenuItem.Header = itemSource;
		}

		protected override MenuItemBase CreateItem(object itemSource)
		{
			var item = _generatorDataTemplateHelper.Load(itemSource);

			SetGenerator(item, this);

			return item;
		}

		protected override void DetachItem(MenuItemBase item, object itemSource)
		{
			if (ReferenceEquals(item.DataContext, itemSource))
				item.ClearValue(FrameworkElement.DataContextProperty);

			if (item is HeaderedMenuItem headeredMenuItem && ReferenceEquals(headeredMenuItem.Header, itemSource)) 
				headeredMenuItem.ClearValue(HeaderedMenuItem.HeaderProperty);
		}

		protected override void DisposeItem(MenuItemBase item, object itemSource)
		{
			item.ClearValue(GeneratorPropertyKey);
		}

		public static MenuItemGenerator GetGenerator(DependencyObject element)
		{
			return (MenuItemGenerator) element.GetValue(GeneratorProperty);
		}

		private void OnItemTemplateChanged()
		{
			_generatorDataTemplateHelper.DataTemplate = ItemTemplate;

			OnGeneratorChanged();
		}

		private static void SetGenerator(DependencyObject element, MenuItemGenerator value)
		{
			element.SetValue(GeneratorPropertyKey, value);
		}

		#endregion
	}
}