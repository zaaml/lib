// <copyright file="ControlExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Extensions
{
  public static class ControlExtensions
  {
    public static readonly DependencyProperty CornerRadiusProperty = DPM.RegisterAttached<CornerRadius>
      ("CornerRadius", typeof (CornerRadius));

    public static void SetCornerRadius(UIElement element, CornerRadius value)
    {
      element.SetValue(CornerRadiusProperty, value);
    }

    public static CornerRadius GetCornerRadius(UIElement element)
    {
      return (CornerRadius) element.GetValue(CornerRadiusProperty);
    }
  }
}
