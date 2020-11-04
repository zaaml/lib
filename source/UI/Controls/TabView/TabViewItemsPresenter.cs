// <copyright file="TabViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	[TemplateContractType(typeof(TabViewItemsPresenterTemplateContract))]
	public class TabViewItemsPresenter : ItemsPresenterBase<TabViewControl, TabViewItem, TabViewItemCollection, TabViewItemsPanel>
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty FooterProperty = DPM.Register<object, TabViewItemsPresenter>
			("Footer");

		public static readonly DependencyProperty FooterTemplateProperty = DPM.Register<DataTemplate, TabViewItemsPresenter>
			("FooterTemplate");

		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, TabViewItemsPresenter>
			("Header");

		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<DataTemplate, TabViewItemsPresenter>
			("HeaderTemplate");

		private static readonly DependencyPropertyKey TabViewControlPropertyKey = DPM.RegisterReadOnly<TabViewControl, TabViewItemsPresenter>
			("TabViewControl");

		public static readonly DependencyProperty TabViewControlProperty = TabViewControlPropertyKey.DependencyProperty;

		#endregion

		#region Ctors

		static TabViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabViewItemsPresenter>();
		}

		public TabViewItemsPresenter()
		{
			this.OverrideStyleKey<TabViewItemsPresenter>();
		}

		#endregion

		#region Properties

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

		internal bool HasHiddenChildren => ItemsHost?.HasHiddenChildren ?? false;

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

		public TabViewControl TabViewControl
		{
			get => (TabViewControl) GetValue(TabViewControlProperty);
			internal set => this.SetReadOnlyValue(TabViewControlPropertyKey, value);
		}

		#endregion

		#region  Methods

		private void AttachItemsHost()
		{
			if (ItemsHost == null)
				return;

			ItemsHost.ItemsPresenter = this;
			ItemsHost.HasHiddenChildrenChanged += OnHasHiddenChildrenChanged;
		}

		private void DetachItemsHost()
		{
			if (ItemsHost == null)
				return;

			ItemsHost.HasHiddenChildrenChanged -= OnHasHiddenChildrenChanged;
			ItemsHost.ItemsPresenter = null;
		}

		private void OnHasHiddenChildrenChanged(object sender, EventArgs eventArgs)
		{
			TabViewControl?.OnHasOverflowedChildrenChanged();
		}

		protected override void OnItemsHostAttached()
		{
			base.OnItemsHostAttached();

			AttachItemsHost();
		}

		protected override void OnItemsHostDetaching()
		{
			DetachItemsHost();

			base.OnItemsHostDetaching();
		}

		#endregion
	}

	public class TabViewItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<TabViewItemsPanel, TabViewItem>
	{
	}
}