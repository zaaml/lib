// <copyright file="SnapOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Snapping
{
	[Flags]
  internal enum SnapOptions
  {
    None = 0,
    Fit = 1,
    Move = 2,
    FitMove = Fit | Move
  }
}