// <copyright file="ScaleTransformAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public sealed class ScaleTransformAnimator : AnimatorBase<ScaleTransform>
  {
    #region Static Fields and Constants

    private static readonly ScaleTransform NullTransform = new ScaleTransform();

    #endregion

    #region Fields

    private readonly ScaleTransform _transform = new ScaleTransform();

    #endregion

    #region  Methods

    protected override ScaleTransform EvaluateCurrent()
    {
      var interpolator = DoubleInterpolator.Instance;

      var start = Start ?? NullTransform;
      var end = End ?? NullTransform;

      _transform.ScaleX = interpolator.Evaluate(start.ScaleX, end.ScaleX, RelativeTime, EasingFunction);
      _transform.ScaleY = interpolator.Evaluate(start.ScaleY, end.ScaleY, RelativeTime, EasingFunction);
      _transform.CenterX = interpolator.Evaluate(start.CenterX, end.CenterX, RelativeTime, EasingFunction);
      _transform.CenterY = interpolator.Evaluate(start.CenterY, end.CenterY, RelativeTime, EasingFunction);

      return _transform;
    }

    #endregion
  }
}