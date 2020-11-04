// <copyright file="PanelExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class PanelExtensions
  {
    #region  Methods

    internal static IEnumerable<UIElement> GetVisualChildren(this Panel panel)
    {
#if !SILVERLIGHT
      return panel.Children.Cast<UIElement>();
#else
			return panel.Children;
#endif
    }

    public static void SetZIndex(this UIElement element, int zindex)
    {
      PanelUtils.SetZIndex(element, zindex);
    }

    #endregion
  }
}