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
	[ContentProperty(nameof(ItemCollection))]
	[TemplateContractType(typeof(MenuItemGroupTemplateContract))]
	public class MenuItemGroup : MenuItemGroupBase<MenuItemBase, MenuItemsPresenterHost, MenuItemsPanel>
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<MenuItemCollection, MenuItemGroup>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<MenuItemGeneratorBase, MenuItemGroup>
			("ItemGenerator", g => g.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		#endregion

		#region Ctors

		static MenuItemGroup()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuItemGroup>();
		}

		public MenuItemGroup()
		{
			this.OverrideStyleKey<MenuItemGroup>();

			ItemCollection = new MenuItemCollection(this);
			MenuItemsPresenter = new MenuItemsPresenter
			{
				Items = ItemCollection,
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

		public MenuItemCollection ItemCollection
		{
			get => (MenuItemCollection) GetValue(ItemCollectionProperty);
			private set => this.SetReadOnlyValue(ItemCollectionPropertyKey, value);
		}

		internal override IMenuItemCollection ItemsCore => ItemCollection;

		protected override MenuItemsPresenterBase<MenuItemBase, MenuItemsPanel> MenuItemsPresenter { get; }

		#endregion

		#region  Methods

		private void OnItemGeneratorChanged(MenuItemGeneratorBase oldGenerator, MenuItemGeneratorBase newGenerator)
		{
			ItemCollection.Generator = newGenerator;
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