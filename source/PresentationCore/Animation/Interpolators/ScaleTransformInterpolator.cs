// <copyright file="ScaleTransformAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class ScaleTransformInterpolator : Interpolator<ScaleTransform>
	{
		public static readonly ScaleTransformInterpolator Instance = new();

		private ScaleTransformInterpolator()
		{
		}

		protected internal override void EvaluateCore(ref ScaleTransform value, ScaleTransform start, ScaleTransform end, double progress)
		{
			var interpolator = DoubleInterpolator.Instance;

			value.ScaleX = interpolator.Evaluate(start.ScaleX, end.ScaleX, progress);
			value.ScaleY = interpolator.Evaluate(start.ScaleY, end.ScaleY, progress);
			value.CenterX = interpolator.Evaluate(start.CenterX, end.CenterX, progress);
			value.CenterY = interpolator.Evaluate(start.CenterY, end.CenterY, progress);
		}
	}
}
