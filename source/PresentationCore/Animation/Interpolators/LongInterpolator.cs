// <copyright file="LongInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class LongInterpolator : InterpolatorBase<long>
  {
    #region Static Fields and Constants

    public static LongInterpolator Instance = new LongInterpolator();

    #endregion

    #region Ctors

    private LongInterpolator()
    {
    }

    #endregion

    #region  Methods

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    protected internal override long EvaluateCore(long start, long end, double progress)
    {
      if (progress == 0.0)
        return start;

      if (progress == 1.0)
        return end;

      var addend = (double) (end - start);
      addend *= progress;
      addend += addend > 0.0 ? 0.5 : -0.5;

      return start + (long) addend;
    }

    #endregion
  }
}