// <copyright file="TableViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TableView
{
	[TemplateContractType(typeof(TableViewItemsPresenterTemplateContract))]
	public class TableViewItemsPresenter : ItemsPresenterBase<TableViewControl, TableViewItem, TableViewItemCollection, TableViewPanel>
	{
		static TableViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TableViewItemsPresenter>();
		}

		public TableViewItemsPresenter()
		{
			this.OverrideStyleKey<TableViewControl>();
		}

		internal TableViewControl TableViewControl { get; set; }

		internal void InvalidatePanel()
		{
			InvalidateMeasure();
			ItemsHost?.InvalidateMeasure();
		}

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
	}

	public class TableViewItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<TableViewPanel, TableViewItem>
	{
	}
}