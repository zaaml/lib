// <copyright file="VisibilityExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class VisibilityExtensions
  {
    #region  Methods

    public static void SetVisibility(this FrameworkElement element, ElementVisibility visibility, Visibility autoVisibility)
    {
      if (element != null)
        element.Visibility = VisibilityUtils.EvaluateElementVisibility(visibility, autoVisibility);
    }

    #endregion
  }
}