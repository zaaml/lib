// <copyright file="MatrixTransformInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class MatrixTransformInterpolator : Interpolator<MatrixTransform>
	{
		public static readonly MatrixTransformInterpolator Instance = new();

		private MatrixTransformInterpolator()
		{
		}

		protected internal override void EvaluateCore(ref MatrixTransform value, MatrixTransform start, MatrixTransform end, double progress)
		{
			value.Matrix = MatrixInterpolator.Instance.Evaluate(start.Matrix, end.Matrix, progress);
		}
	}
}
