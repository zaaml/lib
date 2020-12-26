// <copyright file="SolidColorBrushAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public sealed class SolidColorBrushAnimator : AnimatorBase<SolidColorBrush>
  {
    #region Static Fields and Constants

    private static readonly SolidColorBrush NullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

    #endregion

    #region Fields

    private readonly SolidColorBrush _brush = new SolidColorBrush();

    #endregion

    #region  Methods

    protected override SolidColorBrush EvaluateCurrent()
    {
      var start = Start ?? NullBrush;
      var end = End ?? NullBrush;

      _brush.Color = ColorInterpolator.Instance.Evaluate(start.Color, end.Color, RelativeTime, EasingFunction);
      return _brush;
    }

    #endregion
  }
}