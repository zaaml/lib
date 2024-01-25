// <copyright file="IAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;

namespace Zaaml.PresentationCore.Animation
{
	public interface IAnimationValue
	{
		object Current { get; }

		IEasingFunction EasingFunction { get; set; }

		object End { get; set; }

		double RelativeTime { get; set; }

		object Start { get; set; }
	}

	public interface IAnimationValue<T>
	{
		T Current { get; }

		IEasingFunction EasingFunction { get; set; }

		T End { get; set; }

		bool Invert { get; set; }

		double RelativeTime { get; set; }

		T Start { get; set; }
	}
}
