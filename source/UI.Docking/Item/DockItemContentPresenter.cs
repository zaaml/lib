// <copyright file="DockItemContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
	[TemplateContractType(typeof(DockItemContentPresenterTemplateContract))]
	public class DockItemContentPresenter : TemplateContractControl
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, DockItemContentPresenter>
			("Content");

		static DockItemContentPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DockItemContentPresenter>();
		}

		public DockItemContentPresenter()
		{
			this.OverrideStyleKey<DockItemContentPresenter>();
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}
	}

	public class DockItemContentPresenterTemplateContract : TemplateContract
	{
	}
}