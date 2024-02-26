// <copyright file="AnimationTimeline.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class AnimationTimeline : AssetBase, ITimelineClockCallback
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

		public static readonly DependencyProperty RepeatBehaviorProperty = DPM.Register<RepeatBehavior, AnimationTimeline>
			("RepeatBehavior", d => d.OnRepeatBehaviorPropertyChangedPrivate);

		public static readonly DependencyProperty InitCommandProperty = DPM.Register<AnimationCommand, AnimationTimeline>
			("InitCommand");

		private readonly TimelineClock _clock;
		private Commands _commands;
		private double _relativeTime;

		public event EventHandler Started;
		public event EventHandler Completed;
		public event EventHandler Paused;
		public event EventHandler Resumed;

		internal AnimationTimeline()
		{
			_clock = new TimelineClock(this);
		}

		public double AccelerationRatio
		{
			get => (double)GetValue(AccelerationRatioProperty);
			set => SetValue(AccelerationRatioProperty, value);
		}

		protected virtual double ActualAccelerationRatio => AccelerationRatio;

		protected virtual TimeSpan? ActualBeginTime => BeginTime;

		private Commands ActualCommands => _commands ??= new Commands(this);

		protected virtual double ActualDecelerationRatio => DecelerationRatio;

		protected virtual Duration ActualDuration => Duration;

		protected virtual double ActualSpeedRatio => SpeedRatio;

		public bool AutoReverse
		{
			get => (bool)GetValue(AutoReverseProperty);
			set => SetValue(AutoReverseProperty, value.Box());
		}

		public ICommand BeginCommand => ActualCommands.BeginCommand;

		public TimeSpan? BeginTime
		{
			get => (TimeSpan?)GetValue(BeginTimeProperty);
			set => SetValue(BeginTimeProperty, value);
		}

		public double DecelerationRatio
		{
			get => (double)GetValue(DecelerationRatioProperty);
			set => SetValue(DecelerationRatioProperty, value);
		}

		public Duration Duration
		{
			get => (Duration)GetValue(DurationProperty);
			set => SetValue(DurationProperty, value);
		}

		public AnimationCommand InitCommand
		{
			get => (AnimationCommand)GetValue(InitCommandProperty);
			set => SetValue(InitCommandProperty, value);
		}

		public ICommand PauseCommand => ActualCommands.PauseCommand;

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

		public RepeatBehavior RepeatBehavior
		{
			get => (RepeatBehavior)GetValue(RepeatBehaviorProperty);
			set => SetValue(RepeatBehaviorProperty, value);
		}

		public ICommand ResumeCommand => ActualCommands.ResumeCommand;

		public ICommand SeekCommand => ActualCommands.SeekCommand;

		public double SpeedRatio
		{
			get => (double)GetValue(SpeedRatioProperty);
			set => SetValue(SpeedRatioProperty, value);
		}

		public ICommand StopCommand => ActualCommands.StopCommand;

		internal TimeSpan Time => _clock.Time;

		public void Begin()
		{
			_clock.Begin();
		}

		protected override void EndInitCore()
		{
			base.EndInitCore();

			InitCommand?.Run(this);
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

		private void OnBeginCommandExecuted(object commandParameter)
		{
			Begin();
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

		private void OnPauseCommandExecuted(object commandParameter)
		{
			Pause();
		}

		protected virtual void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
		}

		internal virtual void OnRelativeTimeChanged()
		{
		}

		private void OnRepeatBehaviorPropertyChangedPrivate(RepeatBehavior oldValue, RepeatBehavior newValue)
		{
			UpdateRepeatBehavior();
		}

		private void OnResumeCommandExecuted(object commandParameter)
		{
			Resume();
		}

		protected virtual void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		private void OnSeekCommandExecuted(object commandParameter)
		{
			switch (commandParameter)
			{
				case TimeSpan timeSpan:
					Seek(timeSpan);
					break;
				case Duration { HasTimeSpan: true } duration:
					Seek(duration.TimeSpan);
					break;
				case string str:
					if (TimeSpan.TryParse(str, out var strTimeSpan))
						Seek(strTimeSpan);
					break;
			}
		}

		private void OnSpeedRatioPropertyChangedPrivate()
		{
			UpdateSpeedRatio();
		}

		protected virtual void OnStarted()
		{
			Started?.Invoke(this, EventArgs.Empty);
		}

		private void OnStopCommandExecuted(object commandParameter)
		{
			Stop();
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

		public void SeekRelative(double relativeTime)
		{
			_clock.Seek(_clock.CalculateTime(relativeTime));
		}

		public void SeekRelativeAlignedToLastTick(double relativeTime)
		{
			_clock.SeekAlignedToLastTick(_clock.CalculateTime(relativeTime));
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

		protected void UpdateRepeatBehavior()
		{
			_clock.RepeatBehavior = RepeatBehavior;
		}

		protected void UpdateSpeedRatio()
		{
			_clock.SpeedRatio = ActualSpeedRatio;
		}

		void ITimelineClockCallback.OnTimeChanged(TimelineClock clock)
		{
			RelativeTime = clock.RelativeTime;
		}

		void ITimelineClockCallback.OnCompleted(TimelineClock clock)
		{
			OnCompleted();
		}

		void ITimelineClockCallback.OnStarted(TimelineClock clock)
		{
			OnStarted();
		}

		void ITimelineClockCallback.OnPaused(TimelineClock clock)
		{
			OnPaused();
		}

		void ITimelineClockCallback.OnResumed(TimelineClock clock)
		{
			OnResumed();
		}

		private sealed class Commands
		{
			public Commands(AnimationTimeline timeline)
			{
				BeginCommand = new RelayCommand(timeline.OnBeginCommandExecuted);
				PauseCommand = new RelayCommand(timeline.OnPauseCommandExecuted);
				ResumeCommand = new RelayCommand(timeline.OnResumeCommandExecuted);
				StopCommand = new RelayCommand(timeline.OnStopCommandExecuted);
				SeekCommand = new RelayCommand(timeline.OnSeekCommandExecuted);
			}

			public ICommand BeginCommand { get; }

			public ICommand PauseCommand { get; }

			public ICommand ResumeCommand { get; }

			public ICommand SeekCommand { get; }

			public ICommand StopCommand { get; }
		}
	}
}