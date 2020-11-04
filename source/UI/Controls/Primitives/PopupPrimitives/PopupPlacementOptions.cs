// <copyright file="PopupPlacementOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	[Flags]
  public enum PopupPlacementOptions
  {
    None = 0,
    Fit = 1,
		Move = 2,
		PreservePosition = 4,
    FitMove = Fit | Move
  }
}