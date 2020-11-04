// <copyright file="IntInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class IntInterpolator : InterpolatorBase<int>
  {
    #region Static Fields and Constants

    public static IntInterpolator Instance = new IntInterpolator();

    #endregion

    #region Ctors

    private IntInterpolator()
    {
    }

    #endregion

    #region  Methods

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    protected internal override int EvaluateCore(int start, int end, double progress)
    {
      if (progress == 0.0)
        return start;

      if (progress == 1.0)
        return end;

      var addend = (double) (end - start);
      addend *= progress;
      addend += addend > 0.0 ? 0.5 : -0.5;

      return start + (int) addend;
    }

    #endregion
  }
}