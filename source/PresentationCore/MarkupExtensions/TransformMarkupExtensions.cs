// <copyright file="TransformMarkupExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class TranslateTransformExtension : MarkupExtensionBase
  {
    #region Properties

    public double X { get; set; }
    public double Y { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new TranslateTransform { X = X, Y = Y };
    }

    #endregion
  }
}