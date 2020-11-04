// <copyright file="DecimalInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class DecimalInterpolator : InterpolatorBase<decimal>
  {
    #region Static Fields and Constants

    public static DecimalInterpolator Instance = new DecimalInterpolator();

    #endregion

    #region Ctors

    private DecimalInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override decimal EvaluateCore(decimal start, decimal end, double progress)
    {
      return start + (end - start) * (decimal) progress;
    }

    #endregion
  }
}