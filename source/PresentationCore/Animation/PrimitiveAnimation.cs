// <copyright file="PrimitiveAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public abstract class PrimitiveAnimation<T> : AnimationBase<T>
	{
		protected PrimitiveAnimation(Interpolator<T> interpolator) : base(interpolator)
		{
		}
	}
}
