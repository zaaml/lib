// <copyright file="AnnulusShapeGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Geometries
{
	internal sealed class AnnulusShapeGeometry : InheritanceContextObject
	{
		public static readonly DependencyProperty StartAngleProperty = DPM.Register<double, AnnulusShapeGeometry>
			("StartAngle", d => d.OnStartAnglePropertyChangedPrivate);

		public static readonly DependencyProperty EndAngleProperty = DPM.Register<double, AnnulusShapeGeometry>
			("EndAngle", d => d.OnEndAnglePropertyChangedPrivate);

		public static readonly DependencyProperty InnerRadiusProperty = DPM.Register<double, AnnulusShapeGeometry>
			("InnerRadius", d => d.OnInnerRadiusPropertyChangedPrivate);

		public static readonly DependencyProperty OuterRadiusProperty = DPM.Register<double, AnnulusShapeGeometry>
			("OuterRadius", d => d.OnOuterRadiusPropertyChangedPrivate);

		private static readonly DependencyPropertyKey GeometryPropertyKey = DPM.RegisterReadOnly<Geometry, AnnulusShapeGeometry>
			("Geometry");

		public static readonly DependencyProperty GeometryProperty = GeometryPropertyKey.DependencyProperty;

		private AnnulusGeometry? _annulusGeometry;
		private AnnulusSectorGeometry? _annulusSectorGeometry;
		private CircleGeometry? _circleGeometry;
		private CircleSectorGeometry? _circleSectorGeometry;

		public double EndAngle
		{
			get => (double)GetValue(EndAngleProperty);
			set => SetValue(EndAngleProperty, value);
		}

		public Geometry Geometry
		{
			get => (Geometry)GetValue(GeometryProperty);
			private set => this.SetReadOnlyValue(GeometryPropertyKey, value);
		}

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

		private void OnEndAnglePropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateAnnulus();
		}

		private void OnInnerRadiusPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateAnnulus();
		}

		private void OnOuterRadiusPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateAnnulus();
		}

		private void OnStartAnglePropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateAnnulus();
		}

		private void UpdateAnnulus()
		{
			var annulusKind = AnnulusBuilder.Build(InnerRadius, OuterRadius, StartAngle, EndAngle, ref _circleGeometry, ref _annulusGeometry, ref _circleSectorGeometry, ref _annulusSectorGeometry);
			var geometry = annulusKind switch
			{
				AnnulusKind.None => null,
				AnnulusKind.Circle => _circleGeometry?.Geometry,
				AnnulusKind.Annulus => _annulusGeometry?.Geometry,
				AnnulusKind.CircleSector => _circleSectorGeometry?.Geometry,
				AnnulusKind.AnnulusSector => _annulusSectorGeometry?.Geometry,
				_ => throw new ArgumentOutOfRangeException()
			};

			Geometry = geometry;
		}
	}
}