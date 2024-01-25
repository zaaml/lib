// <copyright file="AnnulusBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal static class AnnulusBuilder
	{
		public static AnnulusKind Build(double innerRadius, double outerRadius, double startAngle, double endAngle,
			ref CircleGeometry? circleGeometry, ref AnnulusGeometry? annulusGeometry, ref CircleSectorGeometry? circleSectorGeometry, ref AnnulusSectorGeometry? annulusSectorGeometry)
		{
			var radius1 = innerRadius;
			var radius2 = outerRadius;

			outerRadius = Math.Max(radius1, radius2);
			innerRadius = Math.Min(radius1, radius2);

			var centerAngle = GeometryUtils.CalcCenterAngle(ref startAngle, ref endAngle);
			var sector = centerAngle.IsCloseTo(360.0) == false;
			var circle = innerRadius.IsCloseTo(0.0);

			if (centerAngle.IsCloseTo(0.0))
				return AnnulusKind.None;

			if (circle)
			{
				if (outerRadius.IsCloseTo(0.0))
					return AnnulusKind.None;

				if (sector)
				{
					circleSectorGeometry ??= new CircleSectorGeometry();

					circleSectorGeometry.Value.Update(outerRadius, startAngle, endAngle);

					return AnnulusKind.CircleSector;
				}

				circleGeometry ??= new CircleGeometry();

				circleGeometry.Value.Update(outerRadius);

				return AnnulusKind.Circle;
			}

			if (sector)
			{
				annulusSectorGeometry ??= new AnnulusSectorGeometry();

				annulusSectorGeometry.Value.Update(innerRadius, outerRadius, startAngle, endAngle);

				return AnnulusKind.AnnulusSector;
			}

			annulusGeometry ??= new AnnulusGeometry();

			annulusGeometry.Value.Update(innerRadius, outerRadius);

			return AnnulusKind.Annulus;
		}
	}
}