// <copyright file="MenuItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

#if SILVERLIGHT
using Zaaml.UI.Extensions;
#endif
namespace Zaaml.UI.Controls.Menu
{
	[ContentProperty(nameof(Items))]
	[TemplateContractType(typeof(MenuItemGroupTemplateContract))]
	public class MenuItemGroup : MenuItemGroupBase<MenuItemBase, MenuItemsPresenterHost, MenuItemsPanel>
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<MenuItemCollection, MenuItemGroup>
			("ItemsInt");

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<MenuItemGeneratorBase, MenuItemGroup>
			("ItemGenerator", g => g.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		#endregion

		#region Ctors

		static MenuItemGroup()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuItemGroup>();
		}

		public MenuItemGroup()
		{
			this.OverrideStyleKey<MenuItemGroup>();

			Items = new MenuItemCollection(this);
			MenuItemsPresenter = new MenuItemsPresenter
			{
				Items = Items,
				ActualOrientation = Orientation.Vertical
			};
		}

		#endregion

		#region Properties

		public MenuItemGeneratorBase ItemGenerator
		{
			get => (MenuItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public MenuItemCollection Items
		{
			get => (MenuItemCollection) GetValue(ItemsProperty);
			private set => this.SetReadOnlyValue(ItemsPropertyKey, value);
		}

		internal override IMenuItemCollection ItemsCore => Items;

		protected override MenuItemsPresenterBase<MenuItemBase, MenuItemsPanel> MenuItemsPresenter { get; }

		#endregion

		#region  Methods

		private void OnItemGeneratorChanged(MenuItemGeneratorBase oldGenerator, MenuItemGeneratorBase newGenerator)
		{
			Items.Generator = newGenerator;
		}

		#endregion
	}


	public class MenuItemGroupTemplateContract : MenuItemGroupTemplateContractBase<MenuItemBase, MenuItemsPresenterHost, MenuItemsPanel>
	{
		#region Properties

		[TemplateContractPart(Required = true)]
		public MenuItemsPresenterHost ItemsPresenterHost { get; set; }

		protected override MenuItemsPresenterHost ItemsPresenterHostCore => ItemsPresenterHost;

		#endregion
	}
}