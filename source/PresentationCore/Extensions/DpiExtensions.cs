// <copyright file="DpiExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
	public static class DpiExtensions
	{
		public static CornerRadius FromDeviceToLogical(this CornerRadius deviceCornerRadius)
		{
			return DpiUtils.DeviceCornerRadiusToLogical(deviceCornerRadius);
		}

		public static Point FromDeviceToLogical(this Point devicePoint)
		{
			return DpiUtils.DevicePixelsToLogical(devicePoint);
		}

		public static Rect FromDeviceToLogical(this Rect deviceRectangle)
		{
			return DpiUtils.DeviceRectToLogical(deviceRectangle);
		}

		public static Size FromDeviceToLogical(this Size deviceSize)
		{
			return DpiUtils.DeviceSizeToLogical(deviceSize);
		}

		public static Thickness FromDeviceToLogical(this Thickness deviceThickness)
		{
			return DpiUtils.DeviceThicknessToLogical(deviceThickness);
		}

		// CornerRadius
		public static CornerRadius FromLogicalToDevice(this CornerRadius cornerRadius)
		{
			return DpiUtils.LogicalCornerRadiusToDevice(cornerRadius);
		}

		// Point
		public static Point FromLogicalToDevice(this Point logicalPoint)
		{
			return DpiUtils.LogicalPixelsToDevice(logicalPoint);
		}

		// Rect
		public static Rect FromLogicalToDevice(this Rect logicalRectangle)
		{
			return DpiUtils.LogicalRectToDevice(logicalRectangle);
		}

		// Size

		public static Size FromLogicalToDevice(this Size logicalSize)
		{
			return DpiUtils.LogicalSizeToDevice(logicalSize);
		}

		// Thickness

		public static Thickness FromLogicalToDevice(this Thickness logicalThickness)
		{
			return DpiUtils.LogicalThicknessToDevice(logicalThickness);
		}
	}
}