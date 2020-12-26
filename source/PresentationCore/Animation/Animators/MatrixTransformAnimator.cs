// <copyright file="MatrixTransformAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public sealed class MatrixTransformAnimator : AnimatorBase<MatrixTransform>
  {
    #region Static Fields and Constants

    private static readonly MatrixTransform NullTransform = new MatrixTransform { Matrix = Matrix.Identity };

    #endregion

    #region Fields

    private readonly MatrixTransform _transform = new MatrixTransform();

    #endregion

    #region  Methods

    protected override MatrixTransform EvaluateCurrent()
    {
      var interpolator = MatrixInterpolator.Instance;

      var start = Start ?? NullTransform;
      var end = End ?? NullTransform;

      _transform.Matrix = interpolator.Evaluate(start.Matrix, end.Matrix, RelativeTime, EasingFunction);

      return _transform;
    }

    #endregion
  }
}