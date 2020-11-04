// <copyright file="SizeAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public sealed class SizeAnimation : PrimitiveAnimationBase<Size>
  {
    #region Properties

    private protected override IInterpolator<Size> Interpolator => SizeInterpolator.Instance;

    #endregion
  }
}