// <copyright file="ToolBarOverflowItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

namespace Zaaml.UI.Controls.ToolBar
{
	[TemplateContractType(typeof(ToolBarOverflowItemsPresenterTemplateContract))]
	public sealed class ToolBarOverflowItemsPresenter : TemplateContractControl
	{
		private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<ToolBarControl, ToolBarOverflowItemsPresenter>
			("ToolBar");

		public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;

		static ToolBarOverflowItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ToolBarOverflowItemsPresenter>();
		}

		public ToolBarOverflowItemsPresenter()
		{
			this.OverrideStyleKey<ToolBarOverflowItemsPresenter>();

			OverflowItems = new ToolBarOverflowItemCollection(this);
		}

		private ToolBarOverflowItemsPanel ItemsHost => TemplateContract.ItemsHost;

		internal ToolBarOverflowItemCollection OverflowItems { get; }

		private ToolBarOverflowItemsPresenterTemplateContract TemplateContract => (ToolBarOverflowItemsPresenterTemplateContract)TemplateContractCore;

		public ToolBarControl ToolBar
		{
			get => (ToolBarControl)GetValue(ToolBarProperty);
			internal set => this.SetReadOnlyValue(ToolBarPropertyKey, value);
		}

		internal void OnItemAttached(OverflowItem<ToolBarItem> item)
		{
		}

		internal void OnItemDetached(OverflowItem<ToolBarItem> item)
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

	public sealed class ToolBarOverflowItemsPanel : StackItemsPanelBase<OverflowItem<ToolBarItem>>
	{
	}

	public sealed class ToolBarOverflowItemsPresenterTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ToolBarOverflowItemsPanel ItemsHost { get; [UsedImplicitly] private set; }
	}
}