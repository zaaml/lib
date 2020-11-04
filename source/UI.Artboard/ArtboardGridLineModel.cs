// <copyright file="ArtboardGridLineModel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardGridLineModel : GridLineModel<ArtboardGridLineCollection, ArtboardGridLine>
	{
		protected override ArtboardGridLineCollection CreateGridLineCollection()
		{
			return new ArtboardGridLineCollection(this);
		}
	}

	public sealed class ArtboardGridLine : GridLine
	{
	}
}