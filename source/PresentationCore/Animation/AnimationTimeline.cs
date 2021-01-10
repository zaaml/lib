// <copyright file="AnimationTimeline.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class AnimationTimeline : AssetBase, ITimeline
	{
		public static readonly DependencyProperty BeginTimeProperty = DPM.Register<TimeSpan?, AnimationTimeline>
			("BeginTime", null, mt => mt.OnBeginTimePropertyChangedPrivate);

		public static readonly DependencyProperty AutoReverseProperty = DPM.Register<bool, AnimationTimeline>
			("AutoReverse", false, mt => mt.OnAutoReversePropertyChangedPrivate);

		public static readonly DependencyProperty DurationProperty = DPM.Register<Duration, AnimationTimeline>
			("Duration", Duration.Automatic, mt => mt.OnDurationPropertyChangedPrivate);

		public static readonly DependencyProperty SpeedRatioProperty = DPM.Register<double, AnimationTimeline>
			("SpeedRatio", 1.0, mt => mt.OnSpeedRatioPropertyChangedPrivate);

		public static readonly DependencyProperty AccelerationRatioProperty = DPM.Register<double, AnimationTimeline>
			("AccelerationRatio", 0.0, mt => mt.OnAccelerationRatioPropertyChangedPrivate);

		public static readonly DependencyProperty DecelerationRatioProperty = DPM.Register<double, AnimationTimeline>
			("DecelerationRatio", 0.0, mt => mt.OnDecelerationRatioPropertyChangedPrivate);

		private readonly TimelineClock _clock;
		private double _relativeTime;

		public event EventHandler Started;
		public event EventHandler Completed;
		public event EventHandler Paused;
		public event EventHandler Resumed;

		internal AnimationTimeline()
		{
			_clock = new TimelineClock(this);

			BeginCommand = new RelayCommand(Begin);
			PauseCommand = new RelayCommand(Pause);
			ResumeCommand = new RelayCommand(Resume);
			StopCommand = new RelayCommand(Stop);
		}

		public double AccelerationRatio
		{
			get => (double) GetValue(AccelerationRatioProperty);
			set => SetValue(AccelerationRatioProperty, value);
		}

		protected virtual double ActualAccelerationRatio => AccelerationRatio;

		protected virtual TimeSpan? ActualBeginTime => BeginTime;

		protected virtual double ActualDecelerationRatio => DecelerationRatio;

		protected virtual Duration ActualDuration => Duration;

		protected virtual double ActualSpeedRatio => SpeedRatio;

		public bool AutoReverse
		{
			get => (bool) GetValue(AutoReverseProperty);
			set => SetValue(AutoReverseProperty, value);
		}

		public ICommand BeginCommand { get; }

		public TimeSpan? BeginTime
		{
			get => (TimeSpan?) GetValue(BeginTimeProperty);
			set => SetValue(BeginTimeProperty, value);
		}

		public double DecelerationRatio
		{
			get => (double) GetValue(DecelerationRatioProperty);
			set => SetValue(DecelerationRatioProperty, value);
		}

		public Duration Duration
		{
			get => (Duration) GetValue(DurationProperty);
			set => SetValue(DurationProperty, value);
		}

		public ICommand PauseCommand { get; }

		internal double RelativeTime
		{
			get => _relativeTime;
			set
			{
				if (_relativeTime.Equals(value))
					return;

				_relativeTime = value;

				OnRelativeTimeChanged();
			}
		}

		public ICommand ResumeCommand { get; }

		public double SpeedRatio
		{
			get => (double) GetValue(SpeedRatioProperty);
			set => SetValue(SpeedRatioProperty, value);
		}

		public ICommand StopCommand { get; }

		public void Begin()
		{
			_clock.Begin();
		}

		private void OnAccelerationRatioPropertyChangedPrivate()
		{
			UpdateAccelerationRatio();
		}

		protected virtual void OnAutoReverseChanged()
		{
		}

		private void OnAutoReversePropertyChangedPrivate()
		{
			_clock.AutoReverse = AutoReverse;

			OnAutoReverseChanged();
		}

		private void OnBeginTimePropertyChangedPrivate()
		{
			UpdateBeginTime();
		}

		protected virtual void OnCompleted()
		{
			Completed?.Invoke(this, EventArgs.Empty);
		}

		private void OnDecelerationRatioPropertyChangedPrivate()
		{
			UpdateDecelerationRatio();
		}

		private void OnDurationPropertyChangedPrivate()
		{
			UpdateDuration();
		}

		protected virtual void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
		}

		internal virtual void OnRelativeTimeChanged()
		{
		}

		protected virtual void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		private void OnSpeedRatioPropertyChangedPrivate()
		{
			UpdateSpeedRatio();
		}

		protected virtual void OnStarted()
		{
			Started?.Invoke(this, EventArgs.Empty);
		}

		public void Pause()
		{
			_clock.Pause();
		}

		public void Resume()
		{
			_clock.Resume();
		}

		public void Seek(TimeSpan offset)
		{
			_clock.Seek(offset);
		}

		public void SeekAlignedToLastTick(TimeSpan offset)
		{
			_clock.SeekAlignedToLastTick(offset);
		}

		public void Stop()
		{
			_clock.Stop();
		}

		protected void UpdateAccelerationRatio()
		{
			_clock.AccelerationRatio = ActualAccelerationRatio;
		}

		protected void UpdateBeginTime()
		{
			_clock.BeginTime = ActualBeginTime ?? TimeSpan.Zero;
		}

		protected void UpdateDecelerationRatio()
		{
			_clock.DecelerationRatio = ActualDecelerationRatio;
		}

		protected void UpdateDuration()
		{
			_clock.Duration = ActualDuration;
		}

		protected void UpdateSpeedRatio()
		{
			_clock.SpeedRatio = ActualSpeedRatio;
		}

		public void OnRelativeTimeChanged(double time)
		{
			RelativeTime = time;
		}

		void ITimeline.OnCompleted()
		{
			OnCompleted();
		}

		void ITimeline.OnStarted()
		{
			OnStarted();
		}

		void ITimeline.OnPaused()
		{
			OnPaused();
		}

		void ITimeline.OnResumed()
		{
			OnResumed();
		}
	}

	internal interface ITimeline
	{
		void OnCompleted();

		void OnPaused();

		void OnRelativeTimeChanged(double time);

		void OnResumed();

		void OnStarted();
	}

	internal class TimelineClock : DependencyObject
	{
		private static readonly DependencyProperty DoubleProperty = DPM.Register<double, TimelineClock>
			("Double");

		private static readonly DependencyProperty RelativeTimeProperty = DPM.Register<double, TimelineClock>
			("RelativeTime", m => m.OnRelativeTimeChanged);

		private readonly ITimeline _animationTimeline;

		private readonly Storyboard _storyboard;

		public TimelineClock(ITimeline animationTimeline)
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

			_storyboard.Completed += (sender, args) => _animationTimeline.OnCompleted();
			_storyboard.CurrentTimeInvalidated += OnCurrentTimeInvalidated;
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
			get => (double) GetValue(RelativeTimeProperty);
			private set => SetValue(RelativeTimeProperty, value);
		}

		public double SpeedRatio
		{
			get => _storyboard.SpeedRatio;
			set => _storyboard.SpeedRatio = value;
		}

		public void Begin()
		{
			_storyboard.Begin();
			_animationTimeline.OnStarted();
		}

		private void OnCurrentTimeInvalidated(object sender, EventArgs e)
		{
			if (Duration.HasTimeSpan == false)
				return;

			RelativeTime = Duration.TimeSpan.TotalMilliseconds.IsZero() ? 1.0 : _storyboard.GetCurrentTime().TotalMilliseconds / Duration.TimeSpan.TotalMilliseconds;
		}

		private void OnRelativeTimeChanged()
		{
			_animationTimeline.OnRelativeTimeChanged(this.GetValue<double>(RelativeTimeProperty));
		}

		public void Pause()
		{
			_storyboard.Pause();
			_animationTimeline.OnPaused();
		}

		public void Resume()
		{
			_storyboard.Resume();
			_animationTimeline.OnResumed();
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