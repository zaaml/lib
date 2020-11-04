// <copyright file="IInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;
using Zaaml.PresentationCore.Animation.Animators;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
  public interface IInterpolator
  {
    #region  Methods

    object Evaluate(object start, object end, double progress);

    #endregion
  }

	public interface IInterpolator<T> : IInterpolator
  {
    #region  Methods

    T Evaluate(T start, T end, double progress);

    #endregion
  }

	public static class InterpolatorExtensions
  {
    #region  Methods

    public static PrimitiveAnimator<T> CreateAnimator<T>(this IInterpolator<T> interpolator)
    {
      return PrimitiveAnimator.Create(interpolator);
    }

    public static T Evaluate<T>(this IInterpolator<T> interpolator, T start, T end, double progress, IEasingFunction easingFunction = null)
    {
      return interpolator.Evaluate(start, end, easingFunction?.Ease(progress) ?? progress);
    }

    #endregion
  }
}