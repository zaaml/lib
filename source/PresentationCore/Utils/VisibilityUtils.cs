// <copyright file="VisibilityUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
  internal static class VisibilityUtils
  {
    #region  Methods

    public static Visibility EvaluateElementVisibility(ElementVisibility visibility, Visibility autoVisibility)
    {
      if (visibility == ElementVisibility.Collapsed)
        return Visibility.Collapsed;

      if (visibility == ElementVisibility.Hidden)
      {
#if SILVERLIGHT
        return Visibility.Collapsed;
#else
        return Visibility.Hidden;
#endif
      }

      return visibility == ElementVisibility.Visible ? Visibility.Visible : autoVisibility;
    }

    #endregion
  }
}