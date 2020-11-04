// <copyright file="MouseEventArgsInt.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Input
{
  internal class MouseEventArgsInt : EventArgs
  {
    private bool _handled;

    #region Ctors

    public MouseEventArgsInt(bool handled, object originalSource)
    {
      _handled = handled;
      OriginalSource = originalSource;
    }

    #endregion

    #region Properties

    public virtual bool Handled
    {
      get => _handled;
      set => _handled = value;
    }

    public object OriginalSource { get; }

    #endregion
  }
}