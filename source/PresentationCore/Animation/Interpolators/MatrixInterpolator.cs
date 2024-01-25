// <copyright file="MatrixInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class MatrixInterpolator : PrimitiveInterpolator<Matrix>
	{
		public static readonly MatrixInterpolator Instance = new();

		private MatrixInterpolator()
		{
		}

		protected internal override Matrix EvaluateCore(Matrix start, Matrix end, double progress)
		{
			var interpolator = DoubleInterpolator.Instance;

			var m11 = interpolator.EvaluateCore(start.M11, end.M11, progress);
			var m12 = interpolator.EvaluateCore(start.M12, end.M12, progress);
			var m21 = interpolator.EvaluateCore(start.M21, end.M21, progress);
			var m22 = interpolator.EvaluateCore(start.M22, end.M22, progress);

			var offsetX = interpolator.EvaluateCore(start.OffsetX, end.OffsetX, progress);
			var offsetY = interpolator.EvaluateCore(start.OffsetY, end.OffsetY, progress);

			return new Matrix(m11, m12, m21, m22, offsetX, offsetY);
		}
	}
}