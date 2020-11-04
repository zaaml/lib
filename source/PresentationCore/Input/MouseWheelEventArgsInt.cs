// <copyright file="MouseWheelEventArgsInt.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Input
{
  internal class MouseWheelEventArgsInt : MouseEventArgsInt
  {
    #region Fields

    #endregion

    #region Ctors

    public MouseWheelEventArgsInt(bool handled, object originalSource, int delta)
      : base(handled, originalSource)
    {
      Delta = delta;
    }

    #endregion

    #region Properties

    public int Delta { get; }

    #endregion
  }
}