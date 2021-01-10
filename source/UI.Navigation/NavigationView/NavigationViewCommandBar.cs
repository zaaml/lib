// <copyright file="NavigationViewCommandBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[ContentProperty(nameof(Items))]
	[TemplateContractType(typeof(NavigationViewCommandBarTemplateContract))]
	public class NavigationViewCommandBar : NavigationViewItemBase
	{
		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<NavigationViewCommandItemCollection, NavigationViewCommandBar>
			("ItemsPrivate");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		static NavigationViewCommandBar()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewCommandBar>();
		}

		public NavigationViewCommandBar()
		{
			this.OverrideStyleKey<NavigationViewCommandBar>();
		}

		public NavigationViewCommandItemCollection Items => this.GetValueOrCreate(ItemsPropertyKey, () => new NavigationViewCommandItemCollection(this));

		private NavigationViewCommandItemsPresenter ItemsPresenter => TemplateContract.ItemsPresenter;

		private NavigationViewCommandBarTemplateContract TemplateContract => (NavigationViewCommandBarTemplateContract) TemplateContractInternal;

		internal void OnItemAttached(NavigationViewCommandItem item)
		{
			item.NavigationViewControl = NavigationViewControl;
			item.ForceCompactDisplayMode = true;
		}

		internal void OnItemDetached(NavigationViewCommandItem item)
		{
			item.NavigationViewControl = null;
			item.ForceCompactDisplayMode = false;
		}

		protected override void OnNavigationViewControlChanged(NavigationViewControl oldNavigationView, NavigationViewControl newNavigationView)
		{
			base.OnNavigationViewControlChanged(oldNavigationView, newNavigationView);

			if (ItemsPresenter != null)
				ItemsPresenter.NavigationViewControl = NavigationViewControl;
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

			ItemsPresenter.NavigationViewControl = NavigationViewControl;
			ItemsPresenter.Items = Items;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.Items = null;
			ItemsPresenter.NavigationViewControl = null;

			base.OnTemplateContractDetaching();
		}
	}
}