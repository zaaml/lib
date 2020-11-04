// <copyright file="PrimitiveAnimationBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Animation.Animators;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  public abstract class PrimitiveAnimationBase<T> : AnimationBase<T>
  {
    #region Properties

    private protected abstract IInterpolator<T> Interpolator { get; }

    #endregion

    #region  Methods

    internal sealed override IAnimator<T> CreateAnimator()
    {
			return new PrimitiveAnimator<T>(Interpolator, From, To)
      {
        EasingFunction = EasingFunction
      };
    }

    #endregion
  }
}