// <copyright file="PopupServiceExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal static class PopupServiceExtensions
  {
    #region  Methods

    internal static void ClosePopupTree(this DependencyObject dependencyObject)
    {
      PopupService.ClosePopupTree(dependencyObject);
    }

    internal static bool IsInPopupTree(this DependencyObject dependencyObject)
    {
      return PopupService.IsInPopupTree(dependencyObject);
    }

    #endregion
  }
}