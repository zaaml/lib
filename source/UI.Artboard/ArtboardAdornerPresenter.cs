// <copyright file="ArtboardAdornerPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardAdornerPresenter : ArtboardComponentControlBase<ArtboardAdornerPanel>
	{
		internal ArtboardAdornerPanel AdornerPanel => TemplateRoot;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			AdornerPanel.Presenter = this;
		}

		protected override void UndoTemplateOverride()
		{
			AdornerPanel.Presenter = this;

			base.UndoTemplateOverride();
		}
	}
}