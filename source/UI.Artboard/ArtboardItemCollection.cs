// <copyright file="ArtboardItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardItemCollection : ItemCollectionBase<ArtboardControl, ArtboardItem>
	{
		internal ArtboardItemCollection(ArtboardControl artboard) : base(artboard)
		{
			Artboard = artboard;
		}

		internal ArtboardControl Artboard { get; }


		protected override ItemGenerator<ArtboardItem> DefaultGenerator => null;
	}
}