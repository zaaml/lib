// <copyright file="TimelineClock.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	internal sealed class TimelineClock : DependencyObject
	{
		private static readonly DependencyProperty DoubleProperty = DPM.Register<double, TimelineClock>
			("Double");

		private static readonly DependencyProperty TimeProperty = DPM.Register<TimeSpan, TimelineClock>
			("Time", m => m.OnTimeChanged);

		private readonly ITimelineClockCallback _animationTimeline;
		private readonly Storyboard _storyboard;

		public TimelineClock(ITimelineClockCallback animationTimeline)
		{
			_animationTimeline = animationTimeline;

			var doubleAnimation = new System.Windows.Media.Animation.DoubleAnimation(1, Duration.Automatic);

			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(DoubleProperty));
			Storyboard.SetTarget(doubleAnimation, this);

			_storyboard = new Storyboard
			{
				Children =
				{
					doubleAnimation
				}
			};

			_storyboard.Completed += OnCompleted;
			_storyboard.CurrentTimeInvalidated += OnCurrentTimeInvalidated;
		}

		public TimelineClock(ITimelineClockCallback animationTimeline, ValueTransition transition) : this(animationTimeline)
		{
			FromTransition(transition);
		}

		public double AccelerationRatio
		{
			get => _storyboard.AccelerationRatio;
			set => _storyboard.AccelerationRatio = value;
		}

		public bool AutoReverse
		{
			get => _storyboard.AutoReverse;
			set => _storyboard.AutoReverse = value;
		}

		public TimeSpan? BeginTime
		{
			get => _storyboard.BeginTime;
			set => _storyboard.BeginTime = value;
		}

		public double DecelerationRatio
		{
			get => _storyboard.DecelerationRatio;
			set => _storyboard.DecelerationRatio = value;
		}

		public Duration Duration
		{
			get => _storyboard.Duration;
			set => _storyboard.Duration = value;
		}

		public double RelativeTime
		{
			get
			{
				if (Duration.HasTimeSpan == false)
					return 0;

				return CalculateRelativeTime(Time);
			}
		}

		public double CalculateRelativeTime(TimeSpan time)
		{
			return Duration.TimeSpan.TotalMilliseconds.IsZero() ? 1.0 : time.TotalMilliseconds / Duration.TimeSpan.TotalMilliseconds;
		}
		
		public TimeSpan CalculateTime(double relativeTime)
		{
			return Duration.TimeSpan.TotalMilliseconds.IsZero() ? TimeSpan.Zero : TimeSpan.FromMilliseconds(relativeTime * Duration.TimeSpan.TotalMilliseconds);
		}

		public double SpeedRatio
		{
			get => _storyboard.SpeedRatio;
			set => _storyboard.SpeedRatio = value;
		}

		public TimeSpan Time
		{
			get => (TimeSpan)GetValue(TimeProperty);
			private set => SetValue(TimeProperty, value);
		}

		public void Begin()
		{
			_storyboard.Begin();
			_animationTimeline.OnStarted(this);
		}

		public void FromTransition(ValueTransition transition)
		{
			Duration = transition.Duration;

			if (transition.AccelerationRatio != 0)
				AccelerationRatio = transition.AccelerationRatio;

			if (transition.DecelerationRatio != 0)
				DecelerationRatio = transition.DecelerationRatio;

			if (transition.SpeedRatio != 0)
				SpeedRatio = transition.SpeedRatio;

			if (transition.BeginTime.HasValue)
				BeginTime = transition.BeginTime;
		}

		private void OnCompleted(object o, EventArgs eventArgs)
		{
			_animationTimeline.OnCompleted(this);
		}

		private void OnCurrentTimeInvalidated(object sender, EventArgs e)
		{
			if (Duration.HasTimeSpan == false)
				return;

			Time = _storyboard.GetCurrentTime();
		}

		private void OnTimeChanged()
		{
			_animationTimeline.OnTimeChanged(this);
		}

		public void Pause()
		{
			_storyboard.Pause();
			_animationTimeline.OnPaused(this);
		}

		public void Resume()
		{
			_storyboard.Resume();
			_animationTimeline.OnResumed(this);
		}

		public void Seek(TimeSpan offset)
		{
			_storyboard.Seek(offset);
		}

		public void SeekAlignedToLastTick(TimeSpan offset)
		{
			_storyboard.SeekAlignedToLastTick(offset);
		}

		public void Stop()
		{
			_storyboard.Stop();
		}
	}
}
