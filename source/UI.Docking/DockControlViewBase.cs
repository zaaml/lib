// <copyright file="DockControlViewBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
	[TemplateContractType(typeof(DockLayoutPresenterTemplateContract))]
	public abstract class DockControlViewBase : TemplateContractControl
	{
		internal AutoHideLayoutView AutoHideLayoutView => TemplateContract.AutoHideLayoutView;

		internal abstract DockControllerBase ControllerCore { get; }

		internal DockLayoutView DockLayoutView => TemplateContract.DockLayoutView;

		internal DocumentLayoutView DocumentLayoutView => TemplateContract.DocumentLayoutView;

		internal FloatLayoutView FloatLayoutView => TemplateContract.FloatLayoutView;

		private DockLayoutPresenterTemplateContract TemplateContract => (DockLayoutPresenterTemplateContract)TemplateContractCore;

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ControllerCore.AutoHideLayout.View = AutoHideLayoutView;
			ControllerCore.DockLayout.View = DockLayoutView;
			ControllerCore.FloatLayout.View = FloatLayoutView;
			ControllerCore.DocumentLayout.View = DocumentLayoutView;

			DocumentLayoutView.RawContent = true;
		}

		protected override void OnTemplateContractDetaching()
		{
			ControllerCore.AutoHideLayout.View = null;
			ControllerCore.DockLayout.View = null;
			ControllerCore.FloatLayout.View = null;
			ControllerCore.DocumentLayout.View = null;

			base.OnTemplateContractDetaching();
		}
	}
}