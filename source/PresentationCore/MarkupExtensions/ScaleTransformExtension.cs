// <copyright file="ScaleTransformExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class ScaleTransformExtension : MarkupExtensionBase
  {
    #region Properties

    public double CenterX { get; set; }

    public double CenterY { get; set; }

    public double ScaleX { get; set; }

    public double ScaleY { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new ScaleTransform { ScaleX = ScaleX, ScaleY = ScaleY, CenterX = CenterX, CenterY = CenterY };
    }

    #endregion
  }
}