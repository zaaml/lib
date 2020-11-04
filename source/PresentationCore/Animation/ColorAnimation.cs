// <copyright file="ColorAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public sealed class ColorAnimation : PrimitiveAnimationBase<Color>
  {
    #region Properties

    private protected override IInterpolator<Color> Interpolator => ColorInterpolator.Instance;

    #endregion
  }
}