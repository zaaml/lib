// <copyright file="WindowHeaderPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
	[TemplateContractType(typeof(WindowHeaderPresenterTemplateContract))]
	public class WindowHeaderPresenter : WindowButtonsElement
	{
		static WindowHeaderPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<WindowHeaderPresenter>();
		}

		public WindowHeaderPresenter()
		{
			this.OverrideStyleKey<WindowHeaderPresenter>();
		}

		private WindowBase WindowBase => (WindowBase)Window;

		protected override void OnWindowAttached()
		{
			base.OnWindowAttached();

			WindowBase.HeaderPresenter = this;
		}

		protected override void OnWindowDetaching()
		{
			WindowBase.HeaderPresenter = null;

			base.OnWindowDetaching();
		}
	}

	public class WindowHeaderPresenterTemplateContract : WindowButtonsElementTemplateContract
	{
	}
}