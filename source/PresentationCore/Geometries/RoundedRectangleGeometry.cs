// <copyright file="RoundedRectangleGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal readonly struct RoundedRectangleGeometry
	{
		private readonly RectangleGeometry _rectangleGeometry;
		private readonly StreamGeometry _streamGeometry;

		public RoundedRectangleGeometry()
		{
			_rectangleGeometry = new RectangleGeometry();
			_streamGeometry = new StreamGeometry();
		}

		public Geometry Update(Rect rect, CornerRadius cornerRadius)
		{
			if (cornerRadius.IsUniform())
			{
				_rectangleGeometry.Rect = rect;
				_rectangleGeometry.RadiusX = cornerRadius.TopLeft;
				_rectangleGeometry.RadiusY = cornerRadius.TopLeft;

				return _rectangleGeometry;
			}

			throw new NotImplementedException();
		}
	}
}