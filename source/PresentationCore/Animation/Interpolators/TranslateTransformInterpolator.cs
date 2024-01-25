// <copyright file="TranslateTransformInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class TranslateTransformInterpolator : Interpolator<TranslateTransform>
	{
		public static readonly TranslateTransformInterpolator Instance = new();

		private TranslateTransformInterpolator()
		{
		}

		protected internal override void EvaluateCore(ref TranslateTransform value, TranslateTransform start, TranslateTransform end, double progress)
		{
			var interpolator = DoubleInterpolator.Instance;

			value.X = interpolator.Evaluate(start.X, end.X, progress);
			value.Y = interpolator.Evaluate(start.Y, end.Y, progress);
		}
	}
}
