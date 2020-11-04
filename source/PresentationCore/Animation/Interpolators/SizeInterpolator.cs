// <copyright file="SizeInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class SizeInterpolator : InterpolatorBase<Size>
  {
    #region Static Fields and Constants

    public static SizeInterpolator Instance = new SizeInterpolator();

    #endregion

    #region Ctors

    private SizeInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override Size EvaluateCore(Size start, Size end, double progress)
    {
      var mw = DoubleInterpolator.Instance.EvaluateCore(start.Width, end.Width, progress);
      var mh = DoubleInterpolator.Instance.EvaluateCore(start.Height, end.Height, progress);
      return new Size(mw, mh);
    }

    #endregion
  }
}