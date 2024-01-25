// <copyright file="IArtboardComponentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	internal interface IArtboardComponentControl
	{
		ArtboardControl Artboard { get; set; }

		double ScrollOffsetX { get; set; }

		double ScrollOffsetY { get; set; }

		double Zoom { get; set; }
	}
}