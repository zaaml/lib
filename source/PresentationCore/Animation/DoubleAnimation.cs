// <copyright file="DoubleAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public sealed class DoubleAnimation : PrimitiveAnimationBase<double>
  {
    #region Properties

    private protected override IInterpolator<double> Interpolator => DoubleInterpolator.Instance;

    #endregion
  }
}