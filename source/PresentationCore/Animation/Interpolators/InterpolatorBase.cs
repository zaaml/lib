// <copyright file="InterpolatorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
  public abstract class InterpolatorBase<T> : IInterpolator<T>
  {
    #region  Methods

    protected internal abstract T EvaluateCore(T start, T end, double progress);

    #endregion

    #region Interface Implementations

    #region IInterpolator

    public object Evaluate(object start, object end, double progress)
    {
      return Evaluate((T) start, (T) end, progress);
    }

    #endregion

    #region IInterpolator<T>

    public T Evaluate(T start, T end, double progress)
    {
      return EvaluateCore(start, end, progress.Clamp(0.0, 1.0));
    }

    #endregion

    #endregion
  }
}