// <copyright file="RibbonToolBarOverflowItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.Overflow;
using Zaaml.UI.Panels.Primitives;

namespace Zaaml.UI.Controls.Ribbon
{
	[TemplateContractType(typeof(RibbonToolBarOverflowItemsPresenterTemplateContract))]
	public sealed class RibbonToolBarOverflowItemsPresenter : TemplateContractControl
	{
		private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<RibbonToolBar, RibbonToolBarOverflowItemsPresenter>
			("ToolBar");

		public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;

		static RibbonToolBarOverflowItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RibbonToolBarOverflowItemsPresenter>();
		}

		public RibbonToolBarOverflowItemsPresenter()
		{
			this.OverrideStyleKey<RibbonToolBarOverflowItemsPresenter>();
			OverflowItems = new RibbonToolBarOverflowItemCollection(this);
		}

		private RibbonToolBarOverflowItemsPanel ItemsHost => TemplateContract.ItemsHost;

		internal RibbonToolBarOverflowItemCollection OverflowItems { get; }

		private RibbonToolBarOverflowItemsPresenterTemplateContract TemplateContract => (RibbonToolBarOverflowItemsPresenterTemplateContract)TemplateContractCore;

		public RibbonToolBar ToolBar
		{
			get => (RibbonToolBar)GetValue(ToolBarProperty);
			internal set => this.SetReadOnlyValue(ToolBarPropertyKey, value);
		}

		internal void OnItemAttached(OverflowItem<RibbonItem> item)
		{
		}

		internal void OnItemDetached(OverflowItem<RibbonItem> item)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			OverflowItems.ItemsHost = ItemsHost;

			base.OnTemplateContractAttached();
		}

		protected override void OnTemplateContractDetaching()
		{
			OverflowItems.ItemsHost = null;

			base.OnTemplateContractDetaching();
		}
	}

	public sealed class RibbonToolBarOverflowItemsPanel : StackItemsPanelBase<OverflowItem<RibbonItem>>
	{
	}

	public sealed class RibbonToolBarOverflowItemsPresenterTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public RibbonToolBarOverflowItemsPanel ItemsHost { get; [UsedImplicitly] private set; }
	}
}