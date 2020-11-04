// <copyright file="SelectNextHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Utils
{
  internal enum SelectDirection
  {
    First,
    Prev,
    Next,
    Last
  }

  internal static class SelectNextHelper
  {
    #region  Methods

    public static int SelectNext(int index, int count, SelectDirection direction, bool circular)
    {
      if (count == 0)
        return -1;
      if (count == 1)
        return 0;

      switch (direction)
      {
        case SelectDirection.First:
          return 0;
        case SelectDirection.Prev:
          if (index > 0)
            return index - 1;
          return circular ? count - 1 : 0;
        case SelectDirection.Next:
          if (index < count - 1)
            return index + 1;
          return circular ? 0 : count - 1;
        case SelectDirection.Last:
          return count - 1;
        default:
          throw new ArgumentOutOfRangeException(nameof(direction));
      }
    }

    #endregion

    //public static void SelectNext<T>(SelectorController<T> selectController, SelectDirection direction, bool circular) where T : FrameworkElement, ISelectable
    //{
    //  selectController.SelectedIndex = SelectNext(selectController.SelectedIndex, selectController.Count, direction, circular);
    //}
  }


}