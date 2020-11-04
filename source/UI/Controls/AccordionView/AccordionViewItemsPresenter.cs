// <copyright file="AccordionViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Primitives;

namespace Zaaml.UI.Controls.AccordionView
{
	[TemplateContractType(typeof(AccordionViewItemsPresenterTemplateContract))]
	public class AccordionViewItemsPresenter : ItemsPresenterBase<AccordionViewControl, AccordionViewItem, AccordionViewItemCollection, AccordionViewPanel>
	{
		#region Ctors

		static AccordionViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<AccordionViewItemsPresenter>();
		}

		public AccordionViewItemsPresenter()
		{
			this.OverrideStyleKey<AccordionViewItemsPresenter>();
		}

		#endregion
	}

	public class AccordionViewPanel : StackItemsPanelBase<AccordionViewItem>
	{
	}

	public class AccordionViewItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<AccordionViewPanel, AccordionViewItem>
	{
	}
}