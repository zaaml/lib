// <copyright file="AnimationTimeline.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class AnimationTimeline : InheritanceContextObject
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

		private readonly AnimationClock _clock;
		private double _time;

		public event EventHandler Started;
		public event EventHandler Completed;
		public event EventHandler Paused;
		public event EventHandler Resumed;

		internal AnimationTimeline()
		{
			_clock = new AnimationClock(this);

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

		public ICommand ResumeCommand { get; }

		public double SpeedRatio
		{
			get => (double) GetValue(SpeedRatioProperty);
			set => SetValue(SpeedRatioProperty, value);
		}

		public ICommand StopCommand { get; }

		internal double Time
		{
			get => _time;
			set
			{
				if (_time.Equals(value))
					return;

				_time = value;

				OnTimeChanged();
			}
		}

		public void Begin()
		{
			_clock.Begin();
		}

		protected virtual void OnAccelerationRatioChanged()
		{
		}

		private void OnAccelerationRatioPropertyChangedPrivate()
		{
			_clock.AccelerationRatio = AccelerationRatio;

			OnAccelerationRatioChanged();
		}

		protected virtual void OnAutoReverseChanged()
		{
		}

		private void OnAutoReversePropertyChangedPrivate()
		{
			_clock.StoryboardAutoReverse = AutoReverse;

			OnAutoReverseChanged();
		}

		protected virtual void OnBeginTimeChanged()
		{
		}

		private void OnBeginTimePropertyChangedPrivate()
		{
			_clock.StoryboardBeginTime = BeginTime;

			OnBeginTimeChanged();
		}

		protected virtual void OnCompleted()
		{
			Completed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnDecelerationRatioChanged()
		{
		}

		private void OnDecelerationRatioPropertyChangedPrivate()
		{
			_clock.DecelerationRatio = DecelerationRatio;

			OnDecelerationRatioChanged();
		}

		protected virtual void OnDurationChanged()
		{
		}

		private void OnDurationPropertyChangedPrivate()
		{
			_clock.StoryboardDuration = Duration;

			OnDurationChanged();
		}

		protected virtual void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnSpeedRatioChanged()
		{
		}

		private void OnSpeedRatioPropertyChangedPrivate()
		{
			_clock.SpeedRatio = SpeedRatio;

			OnSpeedRatioChanged();
		}

		protected virtual void OnStarted()
		{
			Started?.Invoke(this, EventArgs.Empty);
		}

		internal virtual void OnTimeChanged()
		{
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

		private class AnimationClock : DependencyObject
		{
			private static readonly DependencyProperty TimeProperty = DPM.Register<double, AnimationClock>
				("Time", m => m.OnTimeChangedInt);

			private readonly AnimationTimeline _animationTimeline;

			private readonly System.Windows.Media.Animation.DoubleAnimation _doubleAnimation;

			private readonly Storyboard _storyboard;

			public AnimationClock(AnimationTimeline animationTimeline)
			{
				_animationTimeline = animationTimeline;

				_doubleAnimation = new System.Windows.Media.Animation.DoubleAnimation
				{
					From = 0.0,
					To = 1.0
				};

				Storyboard.SetTarget(_doubleAnimation, this);
				Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath(TimeProperty));

				_storyboard = new Storyboard
				{
					Children = {_doubleAnimation}
				};

				_storyboard.Completed += (sender, args) => _animationTimeline.OnCompleted();
			}

			public double AccelerationRatio
			{
				set => _doubleAnimation.AccelerationRatio = value;
			}

			public double DecelerationRatio
			{
				set => _doubleAnimation.DecelerationRatio = value;
			}

			public double SpeedRatio
			{
				set => _doubleAnimation.SpeedRatio = value;
			}

			public bool StoryboardAutoReverse
			{
				set => _doubleAnimation.AutoReverse = value;
			}

			public TimeSpan? StoryboardBeginTime
			{
				set => _doubleAnimation.BeginTime = value;
			}

			public Duration StoryboardDuration
			{
				set => _doubleAnimation.Duration = value;
			}

			public void Begin()
			{
				_storyboard.Begin();
				_animationTimeline.OnStarted();
			}

			private void OnTimeChangedInt()
			{
				_animationTimeline.Time = this.GetValue<double>(TimeProperty);
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
}