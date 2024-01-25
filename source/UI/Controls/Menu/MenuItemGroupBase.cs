// <copyright file="MenuItemGroupBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Menu
{
	public abstract class MenuItemGroupBase : MenuItemBase
	{
		#region Ctors

		internal MenuItemGroupBase()
		{
		}

		#endregion
	}

	public abstract class MenuItemGroupBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel> : MenuItemGroupBase
		where TMenuItem : MenuItemBase
		where TMenuItemsPresenterHost : MenuItemsPresenterHostBase<TMenuItem, TMenuItemsPanel>
		where TMenuItemsPanel : MenuItemsPanelBase<TMenuItem>
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ItemStyleProperty = DPM.Register<Style, MenuItemGroupBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel>>
			(nameof(ItemStyle), g => g.OnItemStyleChanged);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, MenuItemGroupBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel>>
			(nameof(SourceCollection), g => g.OnSourceCollectionChanged);

		public static readonly DependencyProperty DisplayMemberProperty = DPM.Register<string, MenuItemGroupBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel>>
			(nameof(DisplayMember), g => g.OnDisplayMemberPathChangedPrivate);

		#endregion

		#region Properties

		public string DisplayMember
		{
			get => (string) GetValue(DisplayMemberProperty);
			set => SetValue(DisplayMemberProperty, value);
		}

		private TMenuItemsPresenterHost ItemsPresenterHost => TemplateContract.ItemsPresenterHostInternal;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable) GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		public Style ItemStyle
		{
			get => (Style) GetValue(ItemStyleProperty);
			set => SetValue(ItemStyleProperty, value);
		}

		protected abstract MenuItemsPresenterBase<TMenuItem, TMenuItemsPanel> MenuItemsPresenter { get; }

		private MenuItemGroupTemplateContractBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel> TemplateContract => (MenuItemGroupTemplateContractBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel>) TemplateContractCore;

		#endregion

		#region  Methods

		internal virtual void OnDisplayMemberPathChangedInternal(string oldDisplayMemberPath, string newDisplayMemberPath)
		{
		}

		private void OnDisplayMemberPathChangedPrivate(string oldDisplayMemberPath, string newDisplayMemberPath)
		{
			OnDisplayMemberPathChangedInternal(oldDisplayMemberPath, newDisplayMemberPath);
		}

		private void OnSourceCollectionChanged(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemsCore.Source = newSource;
		}

		private void OnItemStyleChanged(Style oldStyle, Style newStyle)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();
			ItemsPresenterHost.ItemsPresenter = MenuItemsPresenter;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenterHost.ItemsPresenter = null;
			base.OnTemplateContractDetaching();
		}

		#endregion
	}

	public abstract class MenuItemGroupTemplateContractBase<TMenuItem, TMenuItemsPresenterHost, TMenuItemsPanel> : MenuItemBaseTemplateContract
		where TMenuItem : MenuItemBase
		where TMenuItemsPresenterHost : MenuItemsPresenterHostBase<TMenuItem, TMenuItemsPanel>
		where TMenuItemsPanel : MenuItemsPanelBase<TMenuItem>
	{
		#region Properties

		protected abstract TMenuItemsPresenterHost ItemsPresenterHostCore { get; }

		internal TMenuItemsPresenterHost ItemsPresenterHostInternal => ItemsPresenterHostCore;

		#endregion
	}
}