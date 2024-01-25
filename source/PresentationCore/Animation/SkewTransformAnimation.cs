// <copyright file="SkewTransformAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SkewTransformAnimation : AnimationBase<SkewTransform>
	{
		public SkewTransformAnimation() : base(SkewTransformInterpolator.Instance)
		{
		}
	}
}
