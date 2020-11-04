// <copyright file="BorderExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Extensions
{
  [Flags]
  public enum BorderStyleProperties
  {
    None = 0x0,
    Background = 0x1,
    BorderBrush = 0x2,
    BorderThickness = 0x4,
    Padding = 0x8,
    All = Background | BorderBrush | BorderThickness | Padding
  }

  public static class BorderExtensions
  {
    public static void BindStyleProperties(this Border border, Control source, BorderStyleProperties properties)
    {
      if (properties.HasFlag(BorderStyleProperties.Background))
        border.SetBinding(Border.BackgroundProperty, new Binding {Path = new PropertyPath(Control.BackgroundProperty), Source = source});
      if (properties.HasFlag(BorderStyleProperties.BorderBrush))
        border.SetBinding(Border.BorderBrushProperty, new Binding { Path = new PropertyPath(Control.BorderBrushProperty), Source = source });
      if (properties.HasFlag(BorderStyleProperties.BorderThickness))
        border.SetBinding(Border.BorderThicknessProperty, new Binding { Path = new PropertyPath(Control.BorderThicknessProperty), Source = source });
      if (properties.HasFlag(BorderStyleProperties.Padding))
        border.SetBinding(Border.PaddingProperty, new Binding { Path = new PropertyPath(Control.PaddingProperty), Source = source });
    }
  }
}
