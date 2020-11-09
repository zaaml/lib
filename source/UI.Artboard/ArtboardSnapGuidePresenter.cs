// <copyright file="ArtboardSnapGuidePresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapGuidePresenter : ArtboardComponentControlBase<ArtboardSnapGuidePanel>
	{
		public ArtboardSnapGuidePresenter()
		{
			SnapGuides = new ArtboardSnapGuideCollectionInternal(this);
		}

		internal ArtboardSnapGuideCollectionInternal SnapGuides { get; }

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			foreach (var snapGuide in SnapGuides)
				TemplateRoot.Children.Add(snapGuide);
		}

		internal void OnSnapGuideAdded(ArtboardSnapGuide snapGuide)
		{
			TemplateRoot?.Children.Add(snapGuide);
		}

		internal void OnSnapGuideRemoved(ArtboardSnapGuide snapGuide)
		{
			TemplateRoot?.Children.Remove(snapGuide);
		}

		protected override void UndoTemplateOverride()
		{
			foreach (var snapGuide in SnapGuides)
				TemplateRoot.Children.Remove(snapGuide);

			base.UndoTemplateOverride();
		}
	}
}