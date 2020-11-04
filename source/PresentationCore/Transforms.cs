// <copyright file="Transforms.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
  public static class Transforms
  {
    #region Static Fields and Constants

    public static readonly Transform RotateCW_90 = new RotateTransform { Angle = 90 }.AsFrozen();
    public static readonly Transform RotateCCW_90 = new RotateTransform { Angle = -90 }.AsFrozen();
    public static readonly Transform Rotate_180 = new RotateTransform { Angle = -180 }.AsFrozen();

    public static readonly Transform FlipHorizontal = new ScaleTransform { ScaleX = -1, ScaleY = 1 }.AsFrozen();
    public static readonly Transform FlipVertical = new ScaleTransform { ScaleX = 1, ScaleY = -1 }.AsFrozen();
    public static readonly Transform FlipBoth = new ScaleTransform { ScaleX = -1, ScaleY = -1 }.AsFrozen();

    public static readonly Transform Identity = new MatrixTransform { Matrix = Matrix.Identity }.AsFrozen();

    #endregion
  }
}