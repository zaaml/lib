// <copyright file="PrimitiveAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
	public class PrimitiveAnimator<T> : AnimatorBase<T>
	{
		#region Fields

		private readonly IInterpolator<T> _interpolator;

		#endregion

		#region Ctors

		public PrimitiveAnimator(IInterpolator<T> interpolator)
		{
			_interpolator = interpolator;
		}

		public PrimitiveAnimator(IInterpolator<T> interpolator, T start, T end)
			: this(interpolator)
		{
			Start = start;
			End = end;
		}

		#endregion

		#region  Methods

		protected override T EvaluateCurrent()
		{
			return _interpolator.Evaluate(Start, End, Time, EasingFunction);
		}

		#endregion
	}

	public static class PrimitiveAnimator
	{
		#region  Methods

		public static PrimitiveAnimator<T> Create<T>(IInterpolator<T> interpolator)
		{
			return new PrimitiveAnimator<T>(interpolator);
		}

		#endregion
	}
}