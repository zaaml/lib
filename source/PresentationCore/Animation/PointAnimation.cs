// <copyright file="PointAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public sealed class PointAnimation : PrimitiveAnimationBase<Point>
  {
    #region Properties

    private protected override IInterpolator<Point> Interpolator => PointInterpolator.Instance;

    #endregion
  }
}