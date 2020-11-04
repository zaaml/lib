// <copyright file="MatrixTransformExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class MatrixTransformExtension : MarkupExtensionBase
  {
    #region Properties

    public Matrix Matrix { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new MatrixTransform { Matrix = Matrix };
    }

    #endregion
  }
}