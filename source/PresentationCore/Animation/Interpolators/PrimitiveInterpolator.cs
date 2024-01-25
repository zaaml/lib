// <copyright file="PrimitiveInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class PrimitiveInterpolator<T> : Interpolator<T>
	{
		public T Evaluate(T start, T end, double progress)
		{
			return EvaluateCore(start, end, progress.Clamp(0.0, 1.0));
		}

		protected internal sealed override void EvaluateCore(ref T value, T start, T end, double progress)
		{
			value = EvaluateCore(start, end, progress);
		}

		protected internal abstract T EvaluateCore(T start, T end, double progress);
	}
}
