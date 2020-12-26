// <copyright file="SkewTransformAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public sealed class SkewTransformAnimator : AnimatorBase<SkewTransform>
  {
    #region Static Fields and Constants

    private static readonly SkewTransform NullTransform = new SkewTransform();

    #endregion

    #region Fields

    private readonly SkewTransform _transform = new SkewTransform();

    #endregion

    #region  Methods

    protected override SkewTransform EvaluateCurrent()
    {
      var interpolator = DoubleInterpolator.Instance;

      var start = Start ?? NullTransform;
      var end = End ?? NullTransform;

      _transform.AngleX = interpolator.Evaluate(start.AngleX, end.AngleX, RelativeTime, EasingFunction);
      _transform.AngleY = interpolator.Evaluate(start.AngleY, end.AngleY, RelativeTime, EasingFunction);
      _transform.CenterX = interpolator.Evaluate(start.CenterX, end.CenterX, RelativeTime, EasingFunction);
      _transform.CenterY = interpolator.Evaluate(start.CenterY, end.CenterY, RelativeTime, EasingFunction);

      return _transform;
    }

    #endregion
  }
}