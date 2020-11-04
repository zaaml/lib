// <copyright file="DpiUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
  internal static class DpiUtils
  {
    #region Static Fields and Constants

    private const BindingFlags DpiBindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
    private static Matrix _toDeviceMatrix;
    private static Matrix _toLogicalMatrix;

    public static readonly int DpiX =
      (int) typeof(SystemParameters).GetProperty("DpiX", DpiBindingFlags).GetValue(null, null);

    public static readonly int DpiY =
      (int) typeof(SystemParameters).GetProperty("Dpi", DpiBindingFlags).GetValue(null, null);

    public static readonly double DpiScaleX = CalcDpiScale(DpiX);
    public static readonly double DpiScaleY = CalcDpiScale(DpiY);

    #endregion

    #region Ctors

    static DpiUtils()
    {
      _toLogicalMatrix = Matrix.Identity;
      _toLogicalMatrix.Scale(96d / DpiX, 96d / DpiY);
      _toDeviceMatrix = Matrix.Identity;
      _toDeviceMatrix.Scale(DpiX / 96d, DpiY / 96d);
    }

    #endregion

    #region  Methods

    private static double CalcDpiScale(int dpi)
    {
      if (dpi != 96)
        return dpi / 96.0;
      return 1.0;
    }

    public static CornerRadius DeviceCornerRadiusToLogical(CornerRadius deviceCornerRadius)
    {
      return _toLogicalMatrix.TransformCornerRadius(deviceCornerRadius);
    }

    public static Point DevicePixelsToLogical(Point devicePoint)
    {
      return _toLogicalMatrix.TransformPoint(devicePoint);
    }

    public static Rect DeviceRectToLogical(Rect deviceRectangle)
    {
      return _toLogicalMatrix.TransformRect(deviceRectangle);
    }

    public static Size DeviceSizeToLogical(Size deviceSize)
    {
      return _toLogicalMatrix.TransformSize(deviceSize);
    }

    public static Thickness DeviceThicknessToLogical(Thickness deviceThickness)
    {
      return _toLogicalMatrix.TransformThickness(deviceThickness);
    }

    // CornerRadius
    public static CornerRadius LogicalCornerRadiusToDevice(CornerRadius cornerRadius)
    {
      return _toDeviceMatrix.TransformCornerRadius(cornerRadius);
    }

    // Point
    public static Point LogicalPixelsToDevice(Point logicalPoint)
    {
      return _toDeviceMatrix.TransformPoint(logicalPoint);
    }

    // Rect
    public static Rect LogicalRectToDevice(Rect logicalRectangle)
    {
      return _toDeviceMatrix.TransformRect(logicalRectangle);
    }

    // Size

    public static Size LogicalSizeToDevice(Size logicalSize)
    {
      return _toDeviceMatrix.TransformSize(logicalSize);
    }

    // Thickness

    public static Thickness LogicalThicknessToDevice(Thickness logicalThickness)
    {
      return _toDeviceMatrix.TransformThickness(logicalThickness);
    }

    #endregion
  }
}