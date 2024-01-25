// <copyright file="DoubleAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class DoubleAnimation : PrimitiveAnimation<double>
	{
		public DoubleAnimation() : base(DoubleInterpolator.Instance)
		{
		}
	}
}