// <copyright file="BackstageViewContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	[TemplateContractType(typeof(BackstageControlContentPresenterTemplateContract))]
	public sealed class BackstageViewContentPresenter : TemplateContractControl
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ContentProperty = DPM.Register<object, BackstageViewContentPresenter>
			("Content");

		private static readonly DependencyPropertyKey BackstageViewControlPropertyKey = DPM.RegisterReadOnly<BackstageViewControl, BackstageViewContentPresenter>
			("BackstageViewControl");

		public static readonly DependencyProperty BackstageViewControlProperty = BackstageViewControlPropertyKey.DependencyProperty;

		#endregion

		#region Ctors

		static BackstageViewContentPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BackstageViewContentPresenter>();
		}

		public BackstageViewContentPresenter()
		{
			this.OverrideStyleKey<BackstageViewContentPresenter>();
		}

		#endregion

		#region Properties

		public BackstageViewControl BackstageViewControl
		{
			get => (BackstageViewControl) GetValue(BackstageViewControlProperty);
			internal set => this.SetReadOnlyValue(BackstageViewControlPropertyKey, value);
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		private BackstageControlContentPresenterTemplateContract TemplateContract => (BackstageControlContentPresenterTemplateContract) TemplateContractCore;

		#endregion
	}

	public class BackstageControlContentPresenterTemplateContract : TemplateContract
	{
		#region Properties

		[TemplateContractPart]
		public ContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

		#endregion
	}
}