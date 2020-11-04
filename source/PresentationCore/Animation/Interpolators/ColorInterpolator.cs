// <copyright file="ColorInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class ColorInterpolator : InterpolatorBase<Color>
  {
    #region Static Fields and Constants

    public static ColorInterpolator Instance = new ColorInterpolator();

    #endregion

    #region Ctors

    private ColorInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override Color EvaluateCore(Color start, Color end, double progress)
    {
      var rgbStartColor = start.ToRgbColor();
      var rgbEndColor = end.ToRgbColor();

      return (rgbStartColor + (rgbEndColor - rgbStartColor) * progress).ToXamlColor();
    }

    #endregion
  }
}