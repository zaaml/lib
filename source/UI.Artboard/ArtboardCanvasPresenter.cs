// <copyright file="ArtboardCanvasPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardCanvasPresenter : FixedTemplateContentControlBase<Panel, ArtboardCanvas>
	{
		public ArtboardCanvas Canvas
		{
			get => ChildCore;
			internal set => ChildCore = value;
		}

		private protected override FixedTemplateContentControlController<Panel, ArtboardCanvas> CreateController()
		{
			return new FixedTemplateContentControlController(this);
		}

		private sealed class FixedTemplateContentControlController : FixedTemplateContentControlController<Panel, ArtboardCanvas>
		{
			public FixedTemplateContentControlController(ArtboardCanvasPresenter canvasPresenter) : base(canvasPresenter)
			{
			}

			private protected override void AttachLogicalChild(ArtboardCanvas child)
			{
				LogicalChildMentor.AttachLogical(child);
			}

			private protected override void DetachLogicalChild(ArtboardCanvas child)
			{
				LogicalChildMentor.DetachLogical(child);
			}
		}
	}
}