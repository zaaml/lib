// <copyright file="SingleInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class SingleInterpolator : InterpolatorBase<float>
  {
    #region Static Fields and Constants

    public static SingleInterpolator Instance = new SingleInterpolator();

    #endregion

    #region Ctors

    private SingleInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override float EvaluateCore(float start, float end, double progress)
    {
      return (float) (start + (end - start) * progress);
    }

    #endregion
  }
}