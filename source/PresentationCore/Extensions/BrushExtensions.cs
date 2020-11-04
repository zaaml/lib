// <copyright file="BrushExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  public static class BrushExtensions
  {
    #region  Methods

    public static SolidColorBrush CloneBrush(this SolidColorBrush brush)
    {
      return BrushUtils.CloneBrush(brush);
    }

    #endregion
  }
}