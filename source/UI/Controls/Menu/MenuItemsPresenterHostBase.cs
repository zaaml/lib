// <copyright file="MenuItemsPresenterHostBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Menu
{
	[ContentProperty(nameof(ItemsPresenter))]
	public abstract class MenuItemsPresenterHostBase<TMenuItem, TMenuItemsPanel> : Panel
		where TMenuItem : MenuItemBase
		where TMenuItemsPanel : MenuItemsPanelBase<TMenuItem>
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ItemsPresenterProperty = DPM.Register<MenuItemsPresenterBase<TMenuItem, TMenuItemsPanel>, MenuItemsPresenterHostBase<TMenuItem, TMenuItemsPanel>>
			("ItemsPresenter", s => s.InvalidateMeasure);

		#endregion

		#region Properties

		public MenuItemsPresenterBase<TMenuItem, TMenuItemsPanel> ItemsPresenter
		{
			get => (MenuItemsPresenterBase<TMenuItem, TMenuItemsPanel>) GetValue(ItemsPresenterProperty);
			set => SetValue(ItemsPresenterProperty, value);
		}

		#endregion

		#region  Methods

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			MountItemContainer();
			return base.ArrangeOverrideCore(finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			MountItemContainer();
			return base.MeasureOverrideCore(availableSize);
		}

		private void MountItemContainer()
		{
			var itemContainer = ItemsPresenter;
			if (itemContainer == null)
				return;

			itemContainer.ItemsPresenterHost = this;
		}

		#endregion
	}
}