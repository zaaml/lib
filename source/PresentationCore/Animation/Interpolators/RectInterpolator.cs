// <copyright file="RectInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class RectInterpolator : InterpolatorBase<Rect>
  {
    #region Static Fields and Constants

    public static RectInterpolator Instance = new RectInterpolator();

    #endregion

    #region Ctors

    private RectInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override Rect EvaluateCore(Rect start, Rect end, double progress)
    {
      var mx = DoubleInterpolator.Instance.EvaluateCore(start.X, end.X, progress);
      var my = DoubleInterpolator.Instance.EvaluateCore(start.Y, end.Y, progress);
      var mw = DoubleInterpolator.Instance.EvaluateCore(start.Width, end.Width, progress);
      var mh = DoubleInterpolator.Instance.EvaluateCore(start.Height, end.Height, progress);
      return new Rect(mx, my, mw, mh);
    }

    #endregion
  }
}