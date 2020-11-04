// <copyright file="FocusNavigationDirectionExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
  internal static class FocusNavigationDirectionExtensions
  {
    #region  Methods

    public static FocusNavigationDirection? FromKey(this Key key, Orientation? orientation = null)
    {
      switch (key)
      {
        case Key.End:
          return FocusNavigationDirection.Last;
        case Key.Home:
          return FocusNavigationDirection.First;
        case Key.Left:
          return orientation == Orientation.Horizontal ? FocusNavigationDirection.Left : FocusNavigationDirection.Previous;
        case Key.Up:
          return orientation == Orientation.Vertical ? FocusNavigationDirection.Up : FocusNavigationDirection.Previous;
        case Key.Right:
          return orientation == Orientation.Horizontal ? FocusNavigationDirection.Right : FocusNavigationDirection.Next;
        case Key.Down:
          return orientation == Orientation.Vertical ? FocusNavigationDirection.Down : FocusNavigationDirection.Next;
      }

      return null;
    }

    #endregion
  }
}