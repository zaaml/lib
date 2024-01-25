// <copyright file="MenuItemsPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.Menu
{
	public abstract class MenuItemsPresenterBase<TMenuItem, TMenuItemsPanel> : Core.ItemsPresenterBase<Control, TMenuItem, MenuItemCollectionBase<TMenuItem>, TMenuItemsPanel>
		where TMenuItem : MenuItemBase
		where TMenuItemsPanel : MenuItemsPanelBase<TMenuItem>
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ActualOrientationPropertyKey = DPM.RegisterReadOnly<Orientation, MenuItemsPresenterBase<TMenuItem, TMenuItemsPanel>>
			(nameof(ActualOrientation));

		public static readonly DependencyProperty ActualOrientationProperty = ActualOrientationPropertyKey.DependencyProperty;

		#endregion

		#region Fields

		private MenuItemsPresenterHostBase<TMenuItem, TMenuItemsPanel> _itemsPresenterHost;
		private MenuScrollViewerWrapper _menuScrollViewer;

		#endregion

		#region Properties

		public Orientation ActualOrientation
		{
			get => (Orientation) GetValue(ActualOrientationProperty);
			internal set => this.SetReadOnlyValue(ActualOrientationPropertyKey, value);
		}

		internal MenuItemsPresenterHostBase<TMenuItem, TMenuItemsPanel> ItemsPresenterHost
		{
			get => _itemsPresenterHost;
			set
			{
				if (ReferenceEquals(_itemsPresenterHost, value))
					return;

				_itemsPresenterHost?.Children.Remove(this);

				_itemsPresenterHost = value;

				_itemsPresenterHost?.Children.Add(this);
			}
		}

		private MenuScrollViewer ScrollViewer => TemplateContract.ScrollViewer;

		private MenuItemsPresenterBaseTemplateContract<TMenuItem, TMenuItemsPanel> TemplateContract => (MenuItemsPresenterBaseTemplateContract<TMenuItem, TMenuItemsPanel>) TemplateContractCore;

		#endregion

		#region  Methods

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			_menuScrollViewer = new MenuScrollViewerWrapper(ScrollViewer);
		}

		protected override void OnTemplateContractDetaching()
		{
			_menuScrollViewer = default(MenuScrollViewerWrapper);

			base.OnTemplateContractDetaching();
		}

		internal void ResetScrollViewer()
		{
			_menuScrollViewer.ScrollToTop();
		}

		#endregion
	}

	public abstract class MenuItemsPresenterBaseTemplateContract<TMenuItem, TMenuPanel> : ItemsPresenterBaseTemplateContract<TMenuPanel, TMenuItem>
		where TMenuItem : MenuItemBase
		where TMenuPanel : MenuItemsPanelBase<TMenuItem>
	{
		#region Properties

		[TemplateContractPart(Required = true)]
		public MenuScrollViewer ScrollViewer { get; [UsedImplicitly] private set; }

		#endregion
	}
}