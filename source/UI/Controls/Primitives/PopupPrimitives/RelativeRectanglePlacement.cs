// <copyright file="RelativeRectanglePlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class RelativeRectanglePlacement : RelativePlacementBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty RectangleProperty = DPM.Register<Rect, RelativeRectanglePlacement>
      ("Rectangle");

    #endregion

    #region Properties

    public Rect Rectangle
    {
      get => (Rect) GetValue(RectangleProperty);
      set => SetValue(RectangleProperty, value);
    }

    #endregion

    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return Rectangle.Offset(TargetScreenBox.GetTopLeft());
    }

    #endregion
  }
}