// <copyright file="DockItemState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  [Flags]
  public enum DockItemState
  {
    Hidden = 0x1,
    Float = 0x2,
    Dock = 0x4,
    Document = 0x8,
    AutoHide = 0x10
  }
}