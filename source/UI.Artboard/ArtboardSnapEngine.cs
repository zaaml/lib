// <copyright file="ArtboardSnapEngine.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	[ContentProperty(nameof(Targets))]
	public class ArtboardSnapEngine : InheritanceContextObject
	{
		private static readonly DependencyPropertyKey ArtboardPropertyKey = DPM.RegisterReadOnly<ArtboardControl, ArtboardSnapEngine>
			("Artboard", d => d.OnArtboardPropertyChangedPrivate);

		public static readonly DependencyProperty ArtboardProperty = ArtboardPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey TargetsPropertyKey = DPM.RegisterReadOnly<ArtboardSnapTargetCollection, ArtboardSnapEngine>
			("TargetsPrivate");

		private static readonly DependencyPropertyKey SourcesPropertyKey = DPM.RegisterAttachedReadOnly<ArtboardSnapSourceCollection, ArtboardSnapEngine>
			("SourcesPrivate");

		public static readonly DependencyProperty StrengthThresholdProperty = DPM.Register<double, ArtboardSnapEngine>
			("StrengthThreshold", 0.25);

		public static readonly DependencyProperty SourcesProperty = SourcesPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TargetsProperty = TargetsPropertyKey.DependencyProperty;

		public ArtboardControl Artboard
		{
			get => (ArtboardControl) GetValue(ArtboardProperty);
			internal set => this.SetReadOnlyValue(ArtboardPropertyKey, value);
		}

		public double StrengthThreshold
		{
			get => (double) GetValue(StrengthThresholdProperty);
			set => SetValue(StrengthThresholdProperty, value);
		}

		public ArtboardSnapTargetCollection Targets => this.GetValueOrCreate(TargetsPropertyKey, CreateElementCollection);

		private static double CalcForce(ArtboardSnapping snapping, ArtboardSnapParameters snapParameters, out Orientation? orientation)
		{
			var artboardZoom = snapParameters.Context.Engine.Artboard?.Zoom ?? 1.0;
			var calcVector = CalcVector(snapping, snapParameters, out orientation);
			var vectorLength = calcVector.Length;
			var snappingStrength = snapping.TargetPrimitive.Target.Strength;

			if (vectorLength.Equals(0.0))
				return 100000.0 * snappingStrength;

			return snappingStrength / (artboardZoom * artboardZoom) / (vectorLength * vectorLength);
		}

		internal static Vector CalcVector(ArtboardSnapping snapping, ArtboardSnapParameters snapParameters)
		{
			return CalcVector(snapping, snapParameters, out _);
		}

		internal static Vector CalcVector(ArtboardSnapping snapping, ArtboardSnapParameters snapParameters, out Orientation? orientation)
		{
			orientation = null;

			return (snapping.SourcePrimitive, snapping.TargetPrimitive) switch
			{
				(ArtboardSnapSourceLine sourceLine, ArtboardSnapTargetLine targetLine) => CreateLineLineVector(sourceLine, targetLine, snapParameters, out orientation),
				(ArtboardSnapSourceLine sourceLine, ArtboardSnapTargetPoint targetPoint) => CreateLinePointVector(sourceLine, targetPoint, snapParameters, out orientation),
				(ArtboardSnapSourcePoint sourcePoint, ArtboardSnapTargetLine targetLine) => CreatePointLineVector(sourcePoint, targetLine, snapParameters, out orientation),
				(ArtboardSnapSourcePoint sourcePoint, ArtboardSnapTargetPoint targetPoint) => CreatePointPointVector(sourcePoint, targetPoint, snapParameters),

				_ => new Vector(0, 0)
			};
		}

		public virtual ArtboardSnapEngineContext CreateContext(ArtboardSnapEngineContextParameters parameters)
		{
			return new ArtboardSnapEngineContext(this, parameters);
		}

		private ArtboardSnapTargetCollection CreateElementCollection()
		{
			return new ArtboardSnapTargetCollection(this);
		}

		private static Vector CreateLineLineVector(ArtboardSnapSourceLine sourceLine, ArtboardSnapTargetLine targetLine, ArtboardSnapParameters snapParameters, out Orientation? orientation)
		{
			orientation = sourceLine.Axis == ArtboardAxis.X ? Orientation.Vertical : Orientation.Horizontal;

			var sourceValue = sourceLine.GetAxisValue(snapParameters);
			var targetValue = targetLine.GetAxisValue(sourceValue, snapParameters.Context);
			var distance = targetValue - sourceValue;

			return sourceLine.Axis == ArtboardAxis.X ? new Vector(0, distance) : new Vector(distance, 0);
		}

		private static Vector CreateLinePointVector(ArtboardSnapSourceLine sourceLine, ArtboardSnapTargetPoint targetPoint, ArtboardSnapParameters snapParameters, out Orientation? orientation)
		{
			orientation = sourceLine.Axis == ArtboardAxis.X ? Orientation.Vertical : Orientation.Horizontal;

			var distance = sourceLine.GetAxisValue(snapParameters);
			var sourceLocation = sourceLine.Axis == ArtboardAxis.X ? new Point(0, distance) : new Point(distance, 0);
			var targetLocation = targetPoint.GetLocation(sourceLocation, snapParameters.Context);

			if (sourceLine.Axis == ArtboardAxis.X)
				sourceLocation.X = targetLocation.X;
			else
				sourceLocation.Y = targetLocation.Y;

			return targetLocation - sourceLocation;
		}

		private static Vector CreatePointLineVector(ArtboardSnapSourcePoint sourcePoint, ArtboardSnapTargetLine targetLine, ArtboardSnapParameters snapParameters, out Orientation? orientation)
		{
			orientation = targetLine.Axis == ArtboardAxis.X ? Orientation.Vertical : Orientation.Horizontal;

			var sourceLocation = sourcePoint.GetLocation(snapParameters);
			var targetLocation = targetLine.Axis == ArtboardAxis.X ? new Point(sourceLocation.X, targetLine.GetAxisValue(sourceLocation.Y, snapParameters.Context)) : new Point(targetLine.GetAxisValue(sourceLocation.X, snapParameters.Context), sourceLocation.Y);

			return targetLocation - sourceLocation;
		}

		private static Vector CreatePointPointVector(ArtboardSnapSourcePoint sourcePoint, ArtboardSnapTargetPoint targetPoint, ArtboardSnapParameters snapParameters)
		{
			var sourceLocation = sourcePoint.GetLocation(snapParameters);
			var targetLocation = targetPoint.GetLocation(sourceLocation, snapParameters.Context);

			return targetLocation - sourceLocation;
		}

		public static ArtboardSnapSourceCollection GetSources(UIElement element)
		{
			return element.GetValueOrCreate(SourcesPropertyKey, () => new ArtboardSnapSourceCollection(element));
		}

		protected virtual IEnumerable<ArtboardSnapSource> GetSourcesCore(UIElement element)
		{
			return GetSources(element);
		}

		internal IEnumerable<ArtboardSnapSource> GetSourcesInternal(UIElement element)
		{
			return GetSourcesCore(element);
		}

		protected virtual IEnumerable<ArtboardSnapTarget> GetTargetsCore(UIElement element)
		{
			return Targets;
		}

		internal IEnumerable<ArtboardSnapTarget> GetTargetsInternal(UIElement element)
		{
			return GetTargetsCore(element);
		}

		private static bool IsHorizontal(Vector vector)
		{
			return vector.Y.Equals(0.0) && vector.X.Equals(0.0) == false;
		}

		private static bool IsVertical(Vector vector)
		{
			return vector.X.Equals(0.0) && vector.Y.Equals(0.0) == false;
		}

		private void OnArtboardPropertyChangedPrivate(ArtboardControl oldValue, ArtboardControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				OnDetached(oldValue);

			if (newValue != null)
				OnAttached(newValue);
		}

		protected virtual void OnAttached(ArtboardControl artboard)
		{
		}

		protected virtual void OnDetached(ArtboardControl artboard)
		{
		}

		public ArtboardSnapResult Snap(ArtboardSnapParameters snapParameters)
		{
			var context = snapParameters.Context;
			var maxForceSearch = new MaxForceSnappingSearch(StrengthThreshold);

			foreach (var sourcePrimitive in context.SourceSnapPrimitives)
			foreach (var targetPrimitive in context.DynamicTargets)
				maxForceSearch.Process(new ArtboardSnapping(sourcePrimitive, targetPrimitive), snapParameters);

			return maxForceSearch.BuildResult(snapParameters);
		}

		private ref struct MaxForceSnappingSearch
		{
			private ArtboardSnapping _maxVerticalForceSnapping;
			private ArtboardSnapping _maxHorizontalForceSnapping;
			private ArtboardSnapping _maxForceSnapping;

			private double _maxVerticalForce;
			private double _maxHorizontalForce;
			private double _maxForce;

			private readonly double _forceThreshold;

			public MaxForceSnappingSearch(double forceThreshold)
			{
				_forceThreshold = forceThreshold;

				_maxForce = 0.0;
				_maxHorizontalForce = 0.0;
				_maxVerticalForce = 0.0;

				_maxForceSnapping = ArtboardSnapping.Empty;
				_maxHorizontalForceSnapping = ArtboardSnapping.Empty;
				_maxVerticalForceSnapping = ArtboardSnapping.Empty;
			}

			public void Process(ArtboardSnapping snapping, ArtboardSnapParameters snapParameters)
			{
				var force = CalcForce(snapping, snapParameters, out var orientation);

				if (force.IsNaN() || force.IsLessThan(_forceThreshold))
					return;

				if (orientation == Orientation.Horizontal)
				{
					if (force.IsGreaterThan(_maxHorizontalForce))
					{
						_maxHorizontalForce = force;
						_maxHorizontalForceSnapping = snapping;
					}
				}
				else if (orientation == Orientation.Vertical)
				{
					if (force.IsGreaterThan(_maxVerticalForce))
					{
						_maxVerticalForce = force;
						_maxVerticalForceSnapping = snapping;
					}
				}
				else
				{
					if (force.IsGreaterThan(_maxForce))
					{
						_maxForce = force;
						_maxForceSnapping = snapping;
					}
				}
			}

			public ArtboardSnapResult BuildResult(ArtboardSnapParameters snapParameters)
			{
				var forceSquared = _maxHorizontalForce * _maxHorizontalForce + _maxVerticalForce * _maxVerticalForce;
				var force = forceSquared.Equals(0.0) ? 0.0 : Math.Sqrt(forceSquared);

				if (_maxForceSnapping.IsEmpty == false && _maxForce.IsGreaterThan(force))
					return new ArtboardSnapResult(snapParameters, _maxForceSnapping, _maxForceSnapping);

				return new ArtboardSnapResult(snapParameters, _maxHorizontalForceSnapping, _maxVerticalForceSnapping);
			}
		}
	}
}