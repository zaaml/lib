// <copyright file="BackstageViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	[TemplateContractType(typeof(BackstageViewItemsPresenterTemplateContract))]
	public class BackstageViewItemsPresenter : ScrollableItemsPresenterBase<BackstageViewControl, BackstageViewItem, BackstageViewItemCollection, BackstageViewItemsPanel>
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty FooterProperty = DPM.Register<object, BackstageViewItemsPresenter>
			("Footer");

		public static readonly DependencyProperty FooterTemplateProperty = DPM.Register<DataTemplate, BackstageViewItemsPresenter>
			("FooterTemplate");

		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, BackstageViewItemsPresenter>
			("Header");

		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<DataTemplate, BackstageViewItemsPresenter>
			("HeaderTemplate");

		private static readonly DependencyPropertyKey BackstageViewControlPropertyKey = DPM.RegisterReadOnly<BackstageViewControl, BackstageViewItemsPresenter>
			("BackstageViewControl");

		public static readonly DependencyProperty BackstageViewControlProperty = BackstageViewControlPropertyKey.DependencyProperty;

		#endregion

		#region Ctors

		static BackstageViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BackstageViewItemsPresenter>();
		}

		public BackstageViewItemsPresenter()
		{
			this.OverrideStyleKey<BackstageViewItemsPresenter>();
		}

		#endregion

		#region Properties

		public BackstageViewControl BackstageViewControl
		{
			get => (BackstageViewControl) GetValue(BackstageViewControlProperty);
			internal set => this.SetReadOnlyValue(BackstageViewControlPropertyKey, value);
		}

		public object Footer
		{
			get => GetValue(FooterProperty);
			set => SetValue(FooterProperty, value);
		}

		public DataTemplate FooterTemplate
		{
			get => (DataTemplate) GetValue(FooterTemplateProperty);
			set => SetValue(FooterTemplateProperty, value);
		}

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public DataTemplate HeaderTemplate
		{
			get => (DataTemplate) GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		#endregion

		#region  Methods

		protected override void OnItemsHostAttached()
		{
			base.OnItemsHostAttached();

			if (ItemsHost != null)
				ItemsHost.ItemsPresenter = this;
		}

		protected override void OnItemsHostDetaching()
		{
			if (ItemsHost != null)
				ItemsHost.ItemsPresenter = null;

			base.OnItemsHostDetaching();
		}

		#endregion
	}

	public class BackstageViewItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<BackstageViewItemsPanel, BackstageViewItem>
	{
	}
}