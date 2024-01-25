// <copyright file="IInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;

namespace Zaaml.PresentationCore.Animation
{
	public interface IInterpolator
	{
		IAnimationValue CreateAnimationValue();
	}

	public interface IInterpolator<T> : IInterpolator
	{
		void Evaluate(ref T value, T start, T end, double progress);
	}

	public static class InterpolatorExtensions
	{
		public static AnimationValue<T> CreateAnimationValue<T>(this Interpolator<T> interpolator)
		{
			return new AnimationValue<T>(interpolator);
		}

		public static void Evaluate<T>(this IInterpolator<T> interpolator, ref T value, T start, T end, double progress, IEasingFunction easingFunction = null)
		{
			interpolator.Evaluate(ref value, start, end, easingFunction?.Ease(progress) ?? progress);
		}
	}
}