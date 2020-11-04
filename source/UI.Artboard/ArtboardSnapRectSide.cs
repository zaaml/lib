// <copyright file="ArtboardSnapRectSide.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Artboard
{
	[Flags]
	public enum ArtboardSnapRectSide
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		TopLeft = Top | Left,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right,
		All = Left | Top | Right | Bottom
	}
}