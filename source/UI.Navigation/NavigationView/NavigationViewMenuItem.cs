// <copyright file="NavigationViewMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[ContentProperty(nameof(Items))]
	[TemplateContractType(typeof(NavigationViewMenuItemTemplateContract))]
	public class NavigationViewMenuItem : NavigationViewHeaderedIconItem
	{
		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, NavigationViewMenuItem>
			("IsOpen", n => n.OnIsOpenPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<NavigationViewSubItemCollection, NavigationViewMenuItem>
			("ItemsPrivate");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		static NavigationViewMenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewMenuItem>();
		}

		public NavigationViewMenuItem()
		{
			this.OverrideStyleKey<NavigationViewMenuItem>();
		}

		private protected override bool IsExpandedVisualState => IsOpen;

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value.Box());
		}

		public NavigationViewSubItemCollection Items => this.GetValueOrCreate(ItemsPropertyKey, () => new NavigationViewSubItemCollection(this));

		private NavigationViewSubItemsPresenter ItemsPresenter => TemplateContract.ItemsPresenter;

		private NavigationViewMenuItemTemplateContract TemplateContract => (NavigationViewMenuItemTemplateContract)TemplateContractCore;

		private protected override void OnClickCore()
		{
			base.OnClickCore();

			this.SetCurrentValueInternal(IsOpenProperty, IsOpen.Box());
		}

		private void OnIsOpenPropertyChangedPrivate()
		{
			UpdateVisualState(true);
		}

		internal void OnItemAttached(NavigationViewItemBase item)
		{
			item.NavigationViewControl = NavigationViewControl;
		}

		internal void OnItemDetached(NavigationViewItemBase item)
		{
			item.NavigationViewControl = null;
		}

		internal override void OnNavigationViewControlChangedInternal(NavigationViewControl oldNavigationView, NavigationViewControl newNavigationView)
		{
			base.OnNavigationViewControlChanged(oldNavigationView, newNavigationView);

			foreach (var commandItem in Items)
				commandItem.NavigationViewControl = newNavigationView;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.Items = Items;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.Items = null;

			base.OnTemplateContractDetaching();
		}
	}
}