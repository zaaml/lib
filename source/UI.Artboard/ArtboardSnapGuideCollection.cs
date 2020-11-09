// <copyright file="ArtboardSnapGuideCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardSnapGuideCollection : DependencyObjectCollectionBase<ArtboardSnapGuide>
	{
		internal ArtboardSnapGuideCollection(ArtboardControl artboard)
		{
			Artboard = artboard;
		}

		public ArtboardControl Artboard { get; }

		protected override void OnItemAdded(ArtboardSnapGuide snapGuide)
		{
			base.OnItemAdded(snapGuide);

			Artboard.OnSnapGuideAdded(snapGuide);
		}

		protected override void OnItemRemoved(ArtboardSnapGuide snapGuide)
		{
			base.OnItemRemoved(snapGuide);

			Artboard.OnSnapGuideRemoved(snapGuide);
		}
	}
}