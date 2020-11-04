// <copyright file="TranslateTransformAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public sealed class TranslateTransformAnimator : AnimatorBase<TranslateTransform>
  {
    #region Static Fields and Constants

    private static readonly TranslateTransform NullTransform = new TranslateTransform();

    #endregion

    #region Fields

    private readonly TranslateTransform _transform = new TranslateTransform();

    #endregion

    #region  Methods

    protected override TranslateTransform EvaluateCurrent()
    {
      var interpolator = DoubleInterpolator.Instance;

      var start = Start ?? NullTransform;
      var end = End ?? NullTransform;

      _transform.X = interpolator.Evaluate(start.X, end.X, Time, EasingFunction);
      _transform.Y = interpolator.Evaluate(start.Y, end.Y, Time, EasingFunction);

      return _transform;
    }

    #endregion
  }
}