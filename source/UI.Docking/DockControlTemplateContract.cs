// <copyright file="DockControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
	public class DockControlTemplateContract : TemplateContract
	{
		[TemplateContractPart]
		public DockControlView ControlView { get; [UsedImplicitly] private set; }

		[TemplateContractPart]
		public GlobalDropCompass GlobalCompass { get; [UsedImplicitly] private set; }

		[TemplateContractPart]
		public LocalDropCompass LocalCompass { get; [UsedImplicitly] private set; }

		[TemplateContractPart]
		public PreviewDockControlView PreviewControlView { get; [UsedImplicitly] private set; }
	}
}