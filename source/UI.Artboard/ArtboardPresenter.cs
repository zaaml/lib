// <copyright file="ArtboardPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardPresenter : ArtboardControlBase<ArtboardPresenterPanel>
	{
		private ArtboardControl _artboard;

		internal ArtboardControl Artboard
		{
			get => _artboard;
			set
			{
				if (ReferenceEquals(_artboard, value))
					return;

				_artboard = value;

				if (TemplateRoot != null)
					TemplateRoot.Artboard = value;
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Artboard = Artboard;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Artboard = null;

			base.UndoTemplateOverride();
		}
	}
}