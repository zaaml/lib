// <copyright file="RotateTransformInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class RotateTransformInterpolator : Interpolator<RotateTransform>
	{
		public static readonly RotateTransformInterpolator Instance = new();

		private RotateTransformInterpolator()
		{
		}

		protected internal override void EvaluateCore(ref RotateTransform value, RotateTransform start, RotateTransform end, double progress)
		{
			var interpolator = DoubleInterpolator.Instance;

			value.Angle = interpolator.Evaluate(start.Angle, end.Angle, progress);
			value.CenterX = interpolator.Evaluate(start.CenterX, end.CenterX, progress);
			value.CenterY = interpolator.Evaluate(start.CenterY, end.CenterY, progress);
		}
	}
}
