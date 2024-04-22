// <copyright file="WindowButtonsElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Windows
{
	[TemplateContractType(typeof(WindowButtonsElementTemplateContract))]
	public abstract class WindowButtonsElement : WindowElement
	{
		public static readonly DependencyProperty ButtonStyleProperty = DPM.Register<Style, WindowButtonsElement>
			("ButtonStyle");

		public static readonly DependencyProperty ButtonsPresenterTemplateProperty = DPM.Register<ControlTemplate, WindowButtonsElement>
			("ButtonsPresenterTemplate");

		protected WindowButtonsElement()
		{
			Focusable = false;
			IsTabStop = false;
		}

		protected WindowButtonsPresenter ButtonsPresenter => TemplateContract.ButtonsPresenter;

		internal WindowButtonsPresenter ButtonsPresenterInternal => ButtonsPresenter;

		public ControlTemplate ButtonsPresenterTemplate
		{
			get => (ControlTemplate)GetValue(ButtonsPresenterTemplateProperty);
			set => SetValue(ButtonsPresenterTemplateProperty, value);
		}

		public Style ButtonStyle
		{
			get => (Style)GetValue(ButtonStyleProperty);
			set => SetValue(ButtonStyleProperty, value);
		}

		private WindowButtonsElementTemplateContract TemplateContract => (WindowButtonsElementTemplateContract)TemplateContractCore;

		internal override IEnumerable<IWindowElement> EnumerateWindowElements()
		{
			if (ButtonsPresenter != null)
				yield return ButtonsPresenter;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (ButtonsPresenter != null)
				ButtonsPresenter.Window = Window;
		}

		protected override void OnTemplateContractDetaching()
		{
			if (ButtonsPresenter != null)
				ButtonsPresenter.Window = null;

			base.OnTemplateContractDetaching();
		}

		protected override void OnWindowAttached()
		{
			base.OnWindowAttached();

			if (ButtonsPresenter != null)
				ButtonsPresenter.Window = Window;
		}

		protected override void OnWindowDetaching()
		{
			if (ButtonsPresenter != null)
				ButtonsPresenter.Window = null;

			base.OnWindowDetaching();
		}
	}

	public class WindowButtonsElementTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public WindowButtonsPresenter ButtonsPresenter { get; [UsedImplicitly] private set; }
	}
}