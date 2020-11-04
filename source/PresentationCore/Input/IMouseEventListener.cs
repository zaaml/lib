// <copyright file="IHwndMouseListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Input
{
  internal interface IMouseEventListener
  {
    #region  Methods

    void OnMouseEvent(MouseEventInfo eventInfo);

    #endregion
  }
}