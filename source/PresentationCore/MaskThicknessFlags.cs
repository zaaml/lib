// <copyright file="MaskThicknessFlags.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore
{
	[Flags]
	public enum MaskThicknessFlags
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		All = Left | Top | Right | Bottom
	}
}