// <copyright file="PointInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class PointInterpolator : InterpolatorBase<Point>
  {
    #region Static Fields and Constants

    public static PointInterpolator Instance = new PointInterpolator();

    #endregion

    #region Ctors

    private PointInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override Point EvaluateCore(Point start, Point end, double progress)
    {
      var mx = DoubleInterpolator.Instance.EvaluateCore(start.X, end.X, progress);
      var my = DoubleInterpolator.Instance.EvaluateCore(start.Y, end.Y, progress);
      return new Point(mx, my);
    }

    #endregion
  }
}