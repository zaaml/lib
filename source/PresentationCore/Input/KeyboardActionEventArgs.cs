// <copyright file="KeyboardActionEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Input
{
  internal class KeyboardActionEventArgs : EventArgs
  {
    #region Fields

    public readonly KeyboardAction Action;

    #endregion

    #region Ctors

    public KeyboardActionEventArgs(KeyboardAction action)
    {
      Action = action;
    }

    #endregion
  }
}