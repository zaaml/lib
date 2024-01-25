// <copyright file="SingleInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SingleInterpolator : PrimitiveInterpolator<float>
	{
		public static readonly SingleInterpolator Instance = new();

		private SingleInterpolator()
		{
		}

		protected internal override float EvaluateCore(float start, float end, double progress)
		{
			return (float)(start + (end - start) * progress);
		}
	}
}