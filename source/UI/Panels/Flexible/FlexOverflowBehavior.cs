// <copyright file="FlexOverflowBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Panels.Flexible
{
  [Flags]
  public enum FlexOverflowBehavior
  {
    None = 0,
    Pin = 0x1,
    Wrap = 0x2,
    Stretch = 0x4,
    Hide = 0x8
  }
}