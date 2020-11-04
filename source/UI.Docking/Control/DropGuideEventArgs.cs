// <copyright file="DropGuideEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  public class DropGuideEventArgs : EventArgs
  {
    #region Ctors

    public DropGuideEventArgs(DropGuideAction dropGuideAction)
    {
      DropGuideAction = dropGuideAction;
    }

    #endregion

    #region Properties

    public DropGuideAction DropGuideAction { get; }

    #endregion
  }
}