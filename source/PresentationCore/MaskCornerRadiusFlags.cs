// <copyright file="MaskCornerRadiusFlags.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore
{
	[Flags]
	public enum MaskCornerRadiusFlags
	{
		None = 0,
		TopLeft = 1,
		TopRight = 2,
		BottomRight = 4,
		BottomLeft = 8,
		All = TopLeft | TopRight | BottomRight | BottomLeft
	}
}