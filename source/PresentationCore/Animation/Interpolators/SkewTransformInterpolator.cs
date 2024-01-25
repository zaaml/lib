// <copyright file="SkewTransformInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SkewTransformInterpolator : Interpolator<SkewTransform>
	{
		public static readonly SkewTransformInterpolator Instance = new();

		private SkewTransformInterpolator()
		{
		}

		protected internal override void EvaluateCore(ref SkewTransform value, SkewTransform start, SkewTransform end, double progress)
		{
			var interpolator = DoubleInterpolator.Instance;

			value.AngleX = interpolator.Evaluate(start.AngleX, end.AngleX, progress);
			value.AngleY = interpolator.Evaluate(start.AngleY, end.AngleY, progress);
			value.CenterX = interpolator.Evaluate(start.CenterX, end.CenterX, progress);
			value.CenterY = interpolator.Evaluate(start.CenterY, end.CenterY, progress);
		}
	}
}
