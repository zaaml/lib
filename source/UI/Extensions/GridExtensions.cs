// <copyright file="GridExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Zaaml.UI.Extensions
{
  internal static class GridExtensions
  {
    #region  Methods

    public static T ApplyGridPosition<T>(this T fre, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1) where T : FrameworkElement
    {
      Grid.SetRow(fre, row);
      Grid.SetColumn(fre, column);
      Grid.SetRowSpan(fre, rowSpan);
      Grid.SetColumnSpan(fre, columnSpan);

      return fre;
    }

    #endregion
  }
}