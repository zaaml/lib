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
		private TableViewItem _footerItem;
		private TableViewItem _headerItem;

		static TableViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TableViewItemsPresenter>();
		}

		public TableViewItemsPresenter()
		{
			this.OverrideStyleKey<TableViewControl>();
		}

		internal TableViewItem FooterItem
		{
			get => _footerItem;
			set
			{
				if (ReferenceEquals(_footerItem, value))
					return;

				_footerItem = value;

				if (ItemsHost != null)
					ItemsHost.FooterItem = _footerItem;
			}
		}

		internal TableViewItem HeaderItem
		{
			get => _headerItem;
			set
			{
				if (ReferenceEquals(_headerItem, value))
					return;

				_headerItem = value;

				if (ItemsHost != null)
					ItemsHost.HeaderItem = _headerItem;
			}
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
			{
				ItemsHost.ItemsPresenter = this;
				ItemsHost.HeaderItem = HeaderItem;
				ItemsHost.FooterItem = FooterItem;
			}
		}

		protected override void OnItemsHostDetaching()
		{
			if (ItemsHost != null)
			{
				ItemsHost.HeaderItem = null;
				ItemsHost.FooterItem = null;
				ItemsHost.ItemsPresenter = null;
			}

			base.OnItemsHostDetaching();
		}
	}
}