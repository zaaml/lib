// <copyright file="PathIcon.Attached.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  public sealed partial class PathIcon
  {
    #region  Methods

    public static Brush GetBrush(DependencyObject element)
    {
      return (Brush) element.GetValue(BrushProperty);
    }

    public static PathIconBrushMode GetBrushMode(DependencyObject element)
    {
      return (PathIconBrushMode) element.GetValue(BrushModeProperty);
    }

    public static Geometry GetData(DependencyObject element)
    {
      return (Geometry) element.GetValue(DataProperty);
    }

    public static Brush GetFill(DependencyObject element)
    {
      return (Brush) element.GetValue(FillProperty);
    }

    public static Stretch GetStretch(DependencyObject element)
    {
      return (Stretch) element.GetValue(StretchProperty);
    }

    public static Brush GetStroke(DependencyObject element)
    {
      return (Brush) element.GetValue(StrokeProperty);
    }

    public static PenLineJoin GetStrokeLineJoin(DependencyObject element)
    {
      return (PenLineJoin) element.GetValue(StrokeLineJoinProperty);
    }

    public static double GetStrokeThickness(DependencyObject element)
    {
      return (double) element.GetValue(StrokeThicknessProperty);
    }

    public static void SetBrush(DependencyObject element, Brush value)
    {
      element.SetValue(BrushProperty, value);
    }

    public static void SetBrushMode(DependencyObject element, PathIconBrushMode value)
    {
      element.SetValue(BrushModeProperty, value);
    }

    public static void SetData(DependencyObject element, Geometry value)
    {
      element.SetValue(DataProperty, value);
    }

    public static void SetFill(DependencyObject element, Brush value)
    {
      element.SetValue(FillProperty, value);
    }

    public static void SetStretch(DependencyObject element, Stretch value)
    {
      element.SetValue(StretchProperty, value);
    }

    public static void SetStroke(DependencyObject element, Brush value)
    {
      element.SetValue(StrokeProperty, value);
    }

    public static void SetStrokeLineJoin(DependencyObject element, PenLineJoin value)
    {
      element.SetValue(StrokeLineJoinProperty, value);
    }

    public static void SetStrokeThickness(DependencyObject element, double value)
    {
      element.SetValue(StrokeThicknessProperty, value);
    }

    #endregion
  }
}