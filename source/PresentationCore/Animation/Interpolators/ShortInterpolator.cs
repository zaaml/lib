// <copyright file="ShortInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class ShortInterpolator : InterpolatorBase<short>
  {
    #region Static Fields and Constants

    public static ShortInterpolator Instance = new ShortInterpolator();

    #endregion

    #region Ctors

    private ShortInterpolator()
    {
    }

    #endregion

    #region  Methods

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    protected internal override short EvaluateCore(short start, short end, double progress)
    {
      if (progress == 0.0)
        return start;

      if (progress == 1.0)
        return end;

      var addend = (double) (end - start);

      addend *= progress;
      addend += addend > 0.0 ? 0.5 : -0.5;

      return (short) (start + (short) addend);
    }

    #endregion
  }
}