// <copyright file="TreeViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	[TemplateContractType(typeof(TreeViewItemsPresenterTemplateContract))]
	public class TreeViewItemsPresenter : ItemsPresenterBase<TreeViewControl, TreeViewItem, TreeViewItemRootCollection, TreeViewPanel>
	{
		static TreeViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItemsPresenter>();
		}

		public TreeViewItemsPresenter()
		{
			this.OverrideStyleKey<TreeViewItemsPresenter>();
		}

		internal TreeViewControl TreeViewControl { get; set; }

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsHost.ItemsPresenter = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsHost.ItemsPresenter = null;

			base.OnTemplateContractDetaching();
		}
	}

	public class TreeViewItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<TreeViewPanel, TreeViewItem>
	{
	}
}