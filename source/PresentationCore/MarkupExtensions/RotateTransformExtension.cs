// <copyright file="RotateTransformExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class RotateTransformExtension : MarkupExtensionBase
  {
    #region Properties

    public double Angle { get; set; }

    public double CenterX { get; set; }

    public double CenterY { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new RotateTransform { Angle = Angle, CenterX = CenterX, CenterY = CenterY };
    }

    #endregion
  }
}