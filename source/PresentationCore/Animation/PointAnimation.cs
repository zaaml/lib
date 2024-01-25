// <copyright file="PointAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class PointAnimation : PrimitiveAnimation<Point>
	{
		public PointAnimation() : base(PointInterpolator.Instance)
		{
		}
	}
}