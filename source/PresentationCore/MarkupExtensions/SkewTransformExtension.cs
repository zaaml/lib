// <copyright file="SkewTransformExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class SkewTransformExtension : MarkupExtensionBase
  {
    #region Properties

    public double AngleX { get; set; }

    public double AngleY { get; set; }

    public double CenterX { get; set; }

    public double CenterY { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new SkewTransform { AngleX = AngleX, AngleY = AngleY, CenterX = CenterX, CenterY = CenterY };
    }

    #endregion
  }
}