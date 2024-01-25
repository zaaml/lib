// <copyright file="RibbonPagesPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
	[TemplateContractType(typeof(RibbonPagesPresenterTemplateContract))]
	public class RibbonPagesPresenter : ItemsPresenterBase<RibbonPageCategory, RibbonPage, RibbonPageCollection, RibbonPagesPanel>
	{
		private static readonly DependencyPropertyKey RibbonPropertyKey = DPM.RegisterReadOnly<RibbonControl, RibbonPagesPresenter>
			("Ribbon", c => c.OnRibbonControlChanged);

		public static readonly DependencyProperty RibbonProperty = RibbonPropertyKey.DependencyProperty;

		static RibbonPagesPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RibbonPagesPresenter>();
		}

		public RibbonPagesPresenter()
		{
			this.OverrideStyleKey<RibbonPagesPresenter>();
		}

		public RibbonControl Ribbon
		{
			get => (RibbonControl)GetValue(RibbonProperty);
			internal set => this.SetReadOnlyValue(RibbonPropertyKey, value);
		}

		private void OnRibbonControlChanged(RibbonControl oldRibbon, RibbonControl newRibbon)
		{
			Items = newRibbon?.Pages;
		}
	}

	public class RibbonPagesPresenterTemplateContract : ItemsPresenterBaseTemplateContract<RibbonPagesPanel, RibbonPage>
	{
	}
}