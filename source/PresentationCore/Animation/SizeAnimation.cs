// <copyright file="SizeAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SizeAnimation : PrimitiveAnimation<Size>
	{
		public SizeAnimation() : base(SizeInterpolator.Instance)
		{
		}
	}
}