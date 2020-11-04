// <copyright file="DoubleInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class DoubleInterpolator : InterpolatorBase<double>
  {
    #region Static Fields and Constants

    public static DoubleInterpolator Instance = new DoubleInterpolator();

    #endregion

    #region Ctors

    private DoubleInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override double EvaluateCore(double start, double end, double progress)
    {
      return start + (end - start) * progress;
    }

    #endregion
  }
}