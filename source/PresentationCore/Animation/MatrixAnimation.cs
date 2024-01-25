// <copyright file="MatrixAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class MatrixAnimation : PrimitiveAnimation<Matrix>
	{
		public MatrixAnimation() : base(MatrixInterpolator.Instance)
		{
		}
	}
}