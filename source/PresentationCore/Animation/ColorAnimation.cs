// <copyright file="ColorAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class ColorAnimation : PrimitiveAnimation<Color>
	{
		public ColorAnimation() : base(ColorInterpolator.Instance)
		{
		}
	}
}