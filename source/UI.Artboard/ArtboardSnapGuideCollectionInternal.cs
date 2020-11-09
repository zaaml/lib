// <copyright file="ArtboardSnapGuideCollectionInternal.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.ObservableCollections;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardSnapGuideCollectionInternal : DispatchedObservableCollection<ArtboardSnapGuide>
	{
		public ArtboardSnapGuideCollectionInternal(ArtboardSnapGuidePresenter presenter)
		{
			Presenter = presenter;
		}

		public ArtboardSnapGuidePresenter Presenter { get; }

		protected override void OnItemAdded(ArtboardSnapGuide snapGuide)
		{
			base.OnItemAdded(snapGuide);

			Presenter.OnSnapGuideAdded(snapGuide);
		}

		protected override void OnItemRemoved(ArtboardSnapGuide snapGuide)
		{
			Presenter.OnSnapGuideRemoved(snapGuide);

			base.OnItemRemoved(snapGuide);
		}
	}
}