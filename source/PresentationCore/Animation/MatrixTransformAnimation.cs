// <copyright file="MatrixTransformAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class MatrixTransformAnimation : AnimationBase<MatrixTransform>
	{
		public MatrixTransformAnimation() : base(MatrixTransformInterpolator.Instance)
		{
		}
	}
}