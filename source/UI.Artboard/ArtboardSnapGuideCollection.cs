// <copyright file="ArtboardSnapGuideItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
	}
}