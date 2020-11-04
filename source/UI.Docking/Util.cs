// <copyright file="Util.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>
using System.Windows.Controls;

namespace Zaaml.UI.Controls.Docking
{
  public static class Util
  {
    #region  Methods

    public static Orientation GetOrientation(Dock side)
    {
      switch (side)
      {
        case Dock.Left:
        case Dock.Right:
          return Orientation.Horizontal;
        case Dock.Top:
        case Dock.Bottom:
          return Orientation.Vertical;
      }

      return Orientation.Horizontal;
    }

    #endregion
  }
}