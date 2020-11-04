// <copyright file="PanelDebug.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Panels.Core
{
#if DEBUG
  public static class PanelDebug
  {
    public static readonly DependencyProperty DebugIdProperty = DependencyProperty.RegisterAttached(
      "DebugId", typeof(string), typeof(PanelDebug), new PropertyMetadata(default(string)));

    public static void SetDebugId(DependencyObject element, string value)
    {
      element.SetValue(DebugIdProperty, value);
    }

    public static string GetDebugId(DependencyObject element)
    {
      return (string) element.GetValue(DebugIdProperty);
    }

    internal static bool ShouldBreak(IPanel panelInterface, string debugId)
    {
      var panel = panelInterface as DependencyObject;
      if (panel == null)
        return false;

      return GetDebugId(panel) == debugId;
    }
  }
#endif
}