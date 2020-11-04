// <copyright file="ScrollCommandKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.ScrollView
{
  public enum ScrollCommandKind
  {
    LineUp,
    LineDown,
    LineLeft,
    LineRight,
    PageUp,
    PageDown,
    PageLeft,
    PageRight,
    MouseWheelUp,
    MouseWheelDown,
    MouseWheelLeft,
    MouseWheelRight,

    ScrollToTop,
    ScrollToBottom,
    ScrollToLeft,
    ScrollToRight,

    ScrollToHome,
    ScrollToEnd
  }
}