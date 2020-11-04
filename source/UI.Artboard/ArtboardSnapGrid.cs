// <copyright file="ArtboardSnapGrid.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Utils;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapGrid : ArtboardSnapTarget
	{
		private readonly List<ArtboardSnapTargetPrimitive> _snapPrimitives;

		public ArtboardSnapGrid()
		{
			_snapPrimitives = new List<ArtboardSnapTargetPrimitive>
			{
				new ArtboardSnapGridPoint(this),
				new ArtboardSnapGridLine(this, ArtboardAxis.X),
				new ArtboardSnapGridLine(this, ArtboardAxis.Y)
			};
		}

		private static double Calc(double value, double gridStep, bool visible)
		{
			return visible ? gridStep * DoubleUtils.Round(value / gridStep, RoundingMode.MidPointFromZero) : value;
		}

		public override IEnumerable<ArtboardSnapTargetPrimitive> GetSnapPrimitives(ArtboardSnapEngineContextParameters parameters)
		{
			return _snapPrimitives;
		}

		private sealed class ArtboardSnapGridPoint : ArtboardSnapTargetPoint
		{
			public ArtboardSnapGridPoint(ArtboardSnapGrid snapGrid) : base(snapGrid)
			{
			}

			public override bool IsFixed => false;

			public override Point GetLocation(Point sourceLocation, ArtboardSnapEngineContext context)
			{
				var gridLineControl = context.Engine.Artboard?.GridLineControlInternal;

				if (gridLineControl == null)
					return new Point(double.NaN, double.NaN);

				var gridStep = gridLineControl.Model.GridSize;

				return new Point(Calc(sourceLocation.X, gridStep, gridLineControl.ShowVerticalLines), Calc(sourceLocation.Y, gridStep, gridLineControl.ShowHorizontalLines));
			}
		}

		private sealed class ArtboardSnapGridLine : ArtboardSnapTargetLine
		{
			public ArtboardSnapGridLine(ArtboardSnapGrid snapGrid, ArtboardAxis axis) : base(snapGrid)
			{
				Axis = axis;
			}

			public override ArtboardAxis Axis { get; }

			public override bool IsFixed => false;

			public override double GetAxisValue(double sourceAxisValue, ArtboardSnapEngineContext context)
			{
				var gridLineControl = context.Engine.Artboard?.GridLineControlInternal;

				if (gridLineControl == null)
					return double.NaN;

				var gridStep = gridLineControl.Model.GridSize;

				return Calc(sourceAxisValue, gridStep, Axis == ArtboardAxis.X ? gridLineControl.ShowVerticalLines : gridLineControl.ShowHorizontalLines);
			}
		}
	}
}