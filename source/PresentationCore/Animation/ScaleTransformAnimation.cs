// <copyright file="ScaleTransformAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class ScaleTransformAnimation : AnimationBase<ScaleTransform>
	{
		public ScaleTransformAnimation() : base(ScaleTransformInterpolator.Instance)
		{
		}
	}
}
