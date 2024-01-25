// <copyright file="AccordionViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.AccordionView
{
	[TemplateContractType(typeof(AccordionViewItemsPresenterTemplateContract))]
	public class AccordionViewItemsPresenter : ItemsPresenterBase<AccordionViewControl, AccordionViewItem, AccordionViewItemCollection, AccordionViewPanel>
	{
		private static readonly DependencyPropertyKey AccordionViewControlPropertyKey = DPM.RegisterReadOnly<AccordionViewControl, AccordionViewItemsPresenter>
			("AccordionViewControl", d => d.OnAccordionViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty AccordionViewControlProperty = AccordionViewControlPropertyKey.DependencyProperty;

		static AccordionViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<AccordionViewItemsPresenter>();
		}

		public AccordionViewItemsPresenter()
		{
			this.OverrideStyleKey<AccordionViewItemsPresenter>();
		}

		public AccordionViewControl AccordionViewControl
		{
			get => (AccordionViewControl)GetValue(AccordionViewControlProperty);
			internal set => this.SetReadOnlyValue(AccordionViewControlPropertyKey, value);
		}

		private void OnAccordionViewControlPropertyChangedPrivate(AccordionViewControl oldValue, AccordionViewControl newValue)
		{
		}
	}
}