//  <copyright file="ResizeHandlerPart.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
  [Flags]
  internal enum ResizableHandleKind
  {
    Undefined = 0x0,
    Left = 0x1,
    Top = 0x2,
    Right = 0x4,
    Bottom = 0x8,
    TopLeft = Top | Left,
    TopRight = Top | Right,
    BottomRight = Bottom | Right,
    BottomLeft = Bottom | Left,
  }
}