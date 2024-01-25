// <copyright file="SeekAnimationCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SeekAnimationCommand : AnimationCommand
	{
		public static readonly DependencyProperty TimeProperty = DPM.Register<TimeSpan, SeekAnimationCommand>
			("Time");

		public static readonly DependencyProperty RelativeTimeProperty = DPM.Register<double, SeekAnimationCommand>
			("RelativeTime", d => d.OnRelativeTimePropertyChangedPrivate, _ => OnCoerceRelativeTime);

		public double RelativeTime
		{
			get => (double)GetValue(RelativeTimeProperty);
			set => SetValue(RelativeTimeProperty, value);
		}

		public TimeSpan Time
		{
			get => (TimeSpan)GetValue(TimeProperty);
			set => SetValue(TimeProperty, value);
		}

		private static double OnCoerceRelativeTime(double relativeTime)
		{
			return relativeTime.Clamp(0, 1);
		}

		private void OnRelativeTimePropertyChangedPrivate(double oldValue, double newValue)
		{
		}

		protected override void RunCore(AnimationTimeline timeline)
		{
			if (this.GetValueSource(TimeProperty) != PropertyValueSource.Default)
				timeline.Seek(Time);
			else if (this.GetValueSource(RelativeTimeProperty) != PropertyValueSource.Default)
				timeline.SeekRelative(RelativeTime);
		}
	}
}