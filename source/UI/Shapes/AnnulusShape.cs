// <copyright file="AnnulusShape.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.PresentationCore.Geometries;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Shapes
{
	public sealed class AnnulusShape : Shape
	{
		public static readonly DependencyProperty StartAngleProperty = DPM.Register<double, AnnulusShape>
			("StartAngle", d => d.OnStartAnglePropertyChangedPrivate);

		public static readonly DependencyProperty EndAngleProperty = DPM.Register<double, AnnulusShape>
			("EndAngle", d => d.OnEndAnglePropertyChangedPrivate);

		public static readonly DependencyProperty InnerRadiusProperty = DPM.Register<double, AnnulusShape>
			("InnerRadius", d => d.OnInnerRadiusPropertyChangedPrivate);

		public static readonly DependencyProperty OuterRadiusProperty = DPM.Register<double, AnnulusShape>
			("OuterRadius", d => d.OnOuterRadiusPropertyChangedPrivate);

		private static readonly Geometry NullGeometry = new RectangleGeometry();

		private AnnulusGeometry? _annulusGeometry;
		private AnnulusSectorGeometry? _annulusSectorGeometry;
		private CircleGeometry? _circleGeometry;
		private CircleSectorGeometry? _circleSectorGeometry;

		protected override Geometry DefiningGeometry => (Geometry ??= UpdateAnnulus()) ?? NullGeometry;

		public double EndAngle
		{
			get => (double)GetValue(EndAngleProperty);
			set => SetValue(EndAngleProperty, value);
		}

		private Geometry Geometry { get; set; }

		public double InnerRadius
		{
			get => (double)GetValue(InnerRadiusProperty);
			set => SetValue(InnerRadiusProperty, value);
		}

		public double OuterRadius
		{
			get => (double)GetValue(OuterRadiusProperty);
			set => SetValue(OuterRadiusProperty, value);
		}

		public double StartAngle
		{
			get => (double)GetValue(StartAngleProperty);
			set => SetValue(StartAngleProperty, value);
		}

		private void InvalidateGeometry()
		{
			Geometry = null;

			InvalidateMeasure();
			InvalidateArrange();
			InvalidateVisual();
		}

		private void OnEndAnglePropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidateGeometry();
		}

		private void OnInnerRadiusPropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidateGeometry();
		}

		private void OnOuterRadiusPropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidateGeometry();
		}

		private void OnStartAnglePropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidateGeometry();
		}

		private Geometry UpdateAnnulus()
		{
			var annulusKind = AnnulusBuilder.Build(InnerRadius, OuterRadius, StartAngle, EndAngle, ref _circleGeometry, ref _annulusGeometry, ref _circleSectorGeometry, ref _annulusSectorGeometry);

			return annulusKind switch
			{
				AnnulusKind.None => null,
				AnnulusKind.Circle => _circleGeometry?.Geometry,
				AnnulusKind.Annulus => _annulusGeometry?.Geometry,
				AnnulusKind.CircleSector => _circleSectorGeometry?.Geometry,
				AnnulusKind.AnnulusSector => _annulusSectorGeometry?.Geometry,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}