// <copyright file="BezierConnectorShape.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.PresentationCore.Geometries;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Shapes
{
	internal class BezierConnectorShape : Shape
	{
		public static readonly DependencyProperty BeginPointProperty = DPM.Register<Point, BezierConnectorShape>
			("BeginPoint", default, d => d.OnBeginPointPropertyChangedPrivate);

		public static readonly DependencyProperty EndPointProperty = DPM.Register<Point, BezierConnectorShape>
			("EndPoint", default, d => d.OnEndPointPropertyChangedPrivate);

		private BezierGeometry _bezierGeometry;

		public BezierConnectorShape()
		{
			_bezierGeometry = new BezierGeometry();
		}

		public Point BeginPoint
		{
			get => (Point)GetValue(BeginPointProperty);
			set => SetValue(BeginPointProperty, value);
		}

		protected override Geometry DefiningGeometry => Geometry ??= UpdateGeometry();

		public Point EndPoint
		{
			get => (Point)GetValue(EndPointProperty);
			set => SetValue(EndPointProperty, value);
		}

		private Geometry Geometry { get; set; }

		private void InvalidateGeometry()
		{
			Geometry = null;

			InvalidateMeasure();
			InvalidateArrange();
			InvalidateVisual();
		}

		private void OnBeginPointPropertyChangedPrivate(Point oldValue, Point newValue)
		{
			InvalidateGeometry();
		}

		private void OnEndPointPropertyChangedPrivate(Point oldValue, Point newValue)
		{
			InvalidateGeometry();
		}

		//protected override void OnMouseMove(MouseEventArgs e)
		//{
		//	base.OnMouseMove(e);

		//	GeometryUtils.GetBezierPoints(ActualBeginPoint, ActualEndPoint, out var p1, out var p2, out var p3, out var p4);

		//	var m = e.GetPosition(this);

		//	ActualMousePoint = GeometryUtils.GetBezierPoint(GeometryUtils.GetClosestPointToCubicBezier(4, 0, 1, 4, m, p1, p2, p3, p4), p1, p2, p3, p4);
		//}

		private Geometry UpdateGeometry()
		{
			_bezierGeometry.Update(BeginPoint, EndPoint);

			return _bezierGeometry.Geometry;
		}
	}
}