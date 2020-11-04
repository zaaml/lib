// <copyright file="MatrixAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public sealed class MatrixAnimation : PrimitiveAnimationBase<Matrix>
  {
    #region Properties

    private protected override IInterpolator<Matrix> Interpolator => MatrixInterpolator.Instance;

    #endregion
  }
}