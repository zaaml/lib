// // <copyright file="TransformExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
// //   Copyright (c) zaaml. All rights reserved.
// // </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Monads;

namespace Zaaml.PresentationCore.Extensions
{
  public static class TransformExtensions
  {
    #region  Methods

    public static Point InverseTransform(this Transform transform, Point point)
    {
      return transform.GetInverseTransform().Transform(point);
    }

    public static Rect InverseTransformBounds(this Transform transform, Rect bounds)
    {
      return transform.GetInverseTransform().TransformBounds(bounds);
    }

    public static TranslateTransform Offset(this TranslateTransform transform, Point offset)
    {
      transform.X += offset.X;
      transform.Y += offset.Y;

      return transform;
    }

    public static TranslateTransform Offset(this TranslateTransform transform, Vector offset)
    {
      transform.X += offset.X;
      transform.Y += offset.Y;

      return transform;
    }

    public static TranslateTransform FromPoint(this TranslateTransform transform, Point point)
    {
      transform.X = point.X;
      transform.Y = point.Y;

      return transform;
    }

    public static TranslateTransform FromVector(this TranslateTransform transform, Vector vector)
    {
      transform.X = vector.X;
      transform.Y = vector.Y;

      return transform;
    }

    public static Point AsPoint(this TranslateTransform transform)
    {
      return new Point(transform.X, transform.Y);
    }

    public static Vector AsVector(this TranslateTransform transform)
    {
      return new Vector(transform.X, transform.Y);
    }

    internal static GeneralTransform GetInverseTransform(this Transform transform)
    {
      return transform?.Inverse ?? Transforms.Identity;
    }
    
    #endregion
  }

  public static class MatrixExtensions
  {
    internal static Matrix GetInvertedMatrix(this Matrix matrix)
    {
#if SILVERLIGHT
      var determinant = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
      var num = 1.0 / determinant;

      return new Matrix(matrix.M22 * num, -matrix.M12 * num, -matrix.M21 * num, matrix.M11 * num, (matrix.M21 * matrix.OffsetY - matrix.OffsetX * matrix.M22) * num, (matrix.OffsetX * matrix.M12 - matrix.M11 * matrix.OffsetY) * num);
#else
      matrix.Invert();

      return matrix;
#endif
    }

    internal static Point TransformPoint(this Matrix transform, Point point)
    {
      return transform.Transform(point);
    }

    internal static Rect TransformRect(this Matrix transform, Rect rect)
    {
      var topLeft = transform.Transform(rect.GetTopLeft());
      var bottomRight = transform.Transform(rect.GetBottomRight());

      return new Rect(topLeft, bottomRight);
    }

    internal static Size TransformSize(this Matrix transform, Size size)
    {
      var point = transform.TransformPoint(new Point(size.Width, size.Height));
      return new Size(point.X, point.Y);
    }

    internal static Thickness TransformThickness(this Matrix transform, Thickness thickness)
    {
      var topLeft = transform.TransformPoint(new Point(thickness.Left, thickness.Top));
      var bottomRight = transform.TransformPoint(new Point(thickness.Right, thickness.Bottom));

      return new Thickness(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
    }

    internal static CornerRadius TransformCornerRadius(this Matrix transform, CornerRadius cornerRadius)
    {
      var top = transform.TransformPoint(new Point(cornerRadius.TopLeft, cornerRadius.TopRight));
      var bottom = transform.TransformPoint(new Point(cornerRadius.BottomLeft, cornerRadius.BottomRight));

      return new CornerRadius(top.X, top.Y, bottom.Y, bottom.X);
    }
  }
}