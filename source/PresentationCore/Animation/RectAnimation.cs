// <copyright file="RectAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class RectAnimation : PrimitiveAnimation<Rect>
	{
		public RectAnimation() : base(RectInterpolator.Instance)
		{
		}
	}
}