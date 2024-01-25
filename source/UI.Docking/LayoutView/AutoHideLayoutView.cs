// <copyright file="AutoHideLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
	[TemplateContractType(typeof(AutoHideLayoutViewTemplateContract))]
	public class AutoHideLayoutView : BaseLayoutView<AutoHideLayout>
	{
		static AutoHideLayoutView()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<AutoHideLayoutView>();
		}

		public AutoHideLayoutView()
		{
			this.OverrideStyleKey<AutoHideLayoutView>();
		}

		private AutoHideTabViewControl TabViewControl => TemplateContract.AutoHideTabViewControl;

		private AutoHideLayoutViewTemplateContract TemplateContract => (AutoHideLayoutViewTemplateContract)TemplateContractInternal;

		protected internal override void ArrangeItems()
		{
		}

		private void AttachItem(DockItem item)
		{
			item.TabViewItem.Content = item;
			TabViewControl?.AddItem(item);
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new AutoHideLayoutViewTemplateContract();
		}

		private void DetachItem(DockItem item)
		{
			item.TabViewItem.Content = null;
			TabViewControl?.RemoveItem(item);
		}

		internal override bool IsItemVisible(DockItem item)
		{
			return item.TabViewItem.IsSelected;
		}

		protected override void OnItemAdded(DockItem item)
		{
			AttachItem(item);
		}

		protected override void OnItemRemoved(DockItem item)
		{
			DetachItem(item);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			foreach (var item in Items)
				AttachItem(item);
		}

		protected override void OnTemplateContractDetaching()
		{
			foreach (var item in Items)
				DetachItem(item);

			base.OnTemplateContractDetaching();
		}
	}

	internal sealed class AutoHideLayoutViewTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public AutoHideTabViewControl AutoHideTabViewControl { get; [UsedImplicitly] private set; }
	}
}