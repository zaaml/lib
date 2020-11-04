// <copyright file="RectAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public sealed class RectAnimation : PrimitiveAnimationBase<Rect>
  {
    #region Properties

    private protected override IInterpolator<Rect> Interpolator => RectInterpolator.Instance;

    #endregion
  }
}