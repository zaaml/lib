// <copyright file="IAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;

namespace Zaaml.PresentationCore.Animation.Animators
{
	public interface IAnimator
  {
    #region Properties

    object Current { get; }

    IEasingFunction EasingFunction { get; set; }

    object End { get; set; }

    object Start { get; set; }

    double RelativeTime { get; set; }

    #endregion
  }

	public interface IAnimator<T>
  {
    #region Properties

    T Current { get; }

    IEasingFunction EasingFunction { get; set; }

    T End { get; set; }

    T Start { get; set; }

    double RelativeTime { get; set; }

    #endregion
  }
}