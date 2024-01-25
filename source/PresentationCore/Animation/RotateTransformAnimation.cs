// <copyright file="RotateTransformAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class RotateTransformAnimation : AnimationBase<RotateTransform>
	{
		public RotateTransformAnimation() : base(RotateTransformInterpolator.Instance)
		{
		}
	}
}
