// <copyright file="NavigationViewCommandItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewCommandItemsPresenterTemplateContract))]
	public class NavigationViewCommandItemsPresenter : NavigationViewItemsPresenterBase<NavigationViewCommandBar, NavigationViewCommandItem, NavigationViewCommandItemCollection, NavigationViewCommandPanel>
	{
		private NavigationViewControl _navigationViewControl;

		static NavigationViewCommandItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewCommandItemsPresenter>();
		}

		public NavigationViewCommandItemsPresenter()
		{
			this.OverrideStyleKey<NavigationViewCommandItemsPresenter>();
		}

		internal NavigationViewControl NavigationViewControl
		{
			get => _navigationViewControl;
			set
			{
				if (ReferenceEquals(_navigationViewControl, value))
					return;

				_navigationViewControl = value;

				if (ItemsHost != null)
					ItemsHost.NavigationViewControl = value;
			}
		}

		protected override void OnItemsHostAttached()
		{
			base.OnItemsHostAttached();

			ItemsHost.NavigationViewControl = NavigationViewControl;
		}

		protected override void OnItemsHostDetaching()
		{
			ItemsHost.NavigationViewControl = null;

			base.OnItemsHostDetaching();
		}
	}
}