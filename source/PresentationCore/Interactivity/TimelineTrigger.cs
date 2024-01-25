// <copyright file="TimelineTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Animation;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("KeyTriggers")]
	public sealed class TimelineTrigger : TriggerBase, ITimelineClockCallback
	{
		private readonly TimelineClock _clock;
		private Duration _duration = Duration.Automatic;
		private TimelineKeyTriggerCollection _keyTriggers;
		private TimeSpan _time;

		private double _timelineTime;
		private TimelineTriggerOffsetKind _offsetKind = TimelineTriggerOffsetKind.Absolute;

		public TimelineTrigger()
		{
			_clock = new TimelineClock(this);
		}

		public double AccelerationRatio
		{
			get => _clock.AccelerationRatio;
			set => _clock.AccelerationRatio = value;
		}

		private IEnumerable<TimelineKeyFrameTriggerBase> ActualKeyTriggers => _keyTriggers ?? Enumerable.Empty<TimelineKeyFrameTriggerBase>();

		public bool AutoReverse { get; set; }

		public TimeSpan? BeginTime
		{
			get => _clock.BeginTime;
			set => _clock.BeginTime = value;
		}

		internal override IEnumerable<InteractivityObject> Children => base.Children.Concat(ActualKeyTriggers);

		public double DecelerationRatio
		{
			get => _clock.DecelerationRatio;
			set => _clock.DecelerationRatio = value;
		}

		public Duration Duration
		{
			get => _duration;
			set
			{
				if (Equals(_duration, value))
					return;

				_duration = value;

				UpdateClockDuration();
			}
		}

		//internal TimeSpan Time { get; private set; }

		internal bool IsReversed { get; private set; }

		internal long Iteration { get; private set; } = -1;

		public TimelineKeyTriggerCollection KeyTriggers => _keyTriggers ??= new TimelineKeyTriggerCollection(this);

		public long RepeatCount { get; set; } = -1;

		public double SpeedRatio
		{
			get => _clock.SpeedRatio;
			set => _clock.SpeedRatio = value;
		}

		internal TimeSpan Time
		{
			get => _time;
			private set
			{
				if (_time.Equals(value))
					return;

				_time = value;

				OnTimeChanged();
			}
		}

		public TimelineTriggerOffsetKind OffsetKind
		{
			get => _offsetKind;
			set
			{
				if (_offsetKind == value)
					return;
				
				_offsetKind = value;
				
				UpdateClockDuration();
			}
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceTrigger = (TimelineTrigger) source;

			_keyTriggers = sourceTrigger._keyTriggers?.DeepCloneCollection<TimelineKeyTriggerCollection, TimelineKeyFrameTriggerBase>(sourceTrigger);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new TimelineTrigger();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			foreach (var keyTrigger in ActualKeyTriggers)
				keyTrigger.Load(root);
		}

		protected override void OnIsEnabledChanged()
		{
			base.OnIsEnabledChanged();

			IsReversed = false;

			var isEnabled = IsEnabled;

			if (RepeatCount == 0)
				return;

			foreach (var trigger in ActualKeyTriggers)
				trigger.IsEnabled = isEnabled;

			UpdateClockDuration();

			if (IsEnabled)
			{
				Iteration = 1;

				_clock.Begin();
			}
			else
			{
				_clock.Stop();

				Iteration = -1;
			}
		}

		internal void OnItemAdded(TimelineKeyFrameTriggerBase item)
		{
			item.TimelineTrigger = this;

			UpdateClockDuration();
		}

		internal void OnItemRemoved(TimelineKeyFrameTriggerBase item)
		{
			item.TimelineTrigger = null;

			UpdateClockDuration();
		}

		private void OnTimeChanged()
		{
			UpdateTriggers();
		}

		internal void OnTriggerOffsetChanged(TimelineKeyFrameTriggerBase trigger)
		{
			UpdateClockDuration();
		}

		private void Repeat()
		{
			if (AutoReverse)
				IsReversed = !IsReversed;

			Iteration++;

			_clock.SeekAlignedToLastTick(TimeSpan.Zero);
		}

		private void Stop()
		{
			_clock.SeekAlignedToLastTick(_clock.Duration.TimeSpan);
			
			IsReversed = false;
			Iteration = -1;
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			foreach (var keyTrigger in ActualKeyTriggers)
				keyTrigger.Unload(root);

			base.UnloadCore(root);
		}

		private TimelineTriggerOffsetKind GetActualOffsetKind(TimelineKeyFrameTriggerBase trigger)
		{
			if (trigger.OffsetKind == TimelineTriggerOffsetKind.Auto)
				return OffsetKind == TimelineTriggerOffsetKind.Auto ? TimelineTriggerOffsetKind.Absolute : OffsetKind;

			return trigger.OffsetKind;
		}
		
		private void UpdateClockDuration()
		{
			if (IsEnabled == false)
				return;

			try
			{
				if (_duration != Duration.Automatic)
				{
					_clock.Duration = _duration;

					return;
				}

				var relative = new Duration(TimeSpan.Zero);
				var absolute = new Duration(TimeSpan.Zero);

				foreach (var keyTrigger in KeyTriggers)
				{
					var relativeTime = relative.TimeSpan;
					var offsetKind = GetActualOffsetKind(keyTrigger);
					
					switch (keyTrigger)
					{
						case KeyFrameTrigger keyFrame:
						{
							if (keyFrame.Offset.HasValue)
							{
								if (offsetKind == TimelineTriggerOffsetKind.Relative)
									relative += keyFrame.Offset.Value;
								else
									absolute = absolute > keyFrame.Offset.Value ? absolute : keyFrame.Offset.Value;
							}

							break;
						}
						case SpanFrameTrigger spanFrame:
						{
							if (spanFrame.Offset.HasValue)
							{
								if (offsetKind == TimelineTriggerOffsetKind.Relative)
									relative += spanFrame.Offset.Value;
								else
								{
									var duration = spanFrame.Offset.Value + spanFrame.Duration;
									
									absolute = absolute > duration ? absolute : duration;
								}
							}

							if (spanFrame.Duration.HasTimeSpan && offsetKind == TimelineTriggerOffsetKind.Relative)
								relative += spanFrame.Duration.TimeSpan;

							break;
						}
					}

					if (offsetKind == TimelineTriggerOffsetKind.Relative)
						keyTrigger.ActualOffset = relativeTime + (keyTrigger.Offset ?? TimeSpan.Zero);
					else
						keyTrigger.ActualOffset = keyTrigger.Offset ?? TimeSpan.Zero;
				}

				_clock.Duration = absolute > relative ? absolute : relative;
			}
			finally
			{
				UpdateTime();
			}
		}

		private void UpdateTime()
		{
			var timeSpanOffset = TimeSpan.FromMilliseconds(_clock.Duration.TimeSpan.TotalMilliseconds * _timelineTime);

			Time = IsReversed ? _clock.Duration.TimeSpan - timeSpanOffset : timeSpanOffset;
		}

		private void UpdateTriggers()
		{
			if (Iteration == -1)
				return;

			foreach (var keyTrigger in ActualKeyTriggers)
				keyTrigger.UpdateState();
		}

		void ITimelineClockCallback.OnTimeChanged(TimelineClock clock)
		{
			_timelineTime = clock.RelativeTime;

			UpdateTime();
		}
		
		private bool CompletedHandling { get; set; }

		void ITimelineClockCallback.OnCompleted(TimelineClock clock)
		{
			if (CompletedHandling || Iteration == -1 || IsEnabled == false)
				return;

			try
			{
				CompletedHandling = true;
				
				if (AutoReverse)
				{
					if (RepeatCount > 0 && Iteration == 2 * RepeatCount)
						Stop();
					else
						Repeat();
				}
				else
				{
					if (RepeatCount > 0 && Iteration == RepeatCount)
						Stop();
					else
						Repeat();
				}
			}
			finally
			{
				CompletedHandling = false;
			}
		}

		void ITimelineClockCallback.OnPaused(TimelineClock clock)
		{
		}

		void ITimelineClockCallback.OnResumed(TimelineClock clock)
		{
		}

		void ITimelineClockCallback.OnStarted(TimelineClock clock)
		{
		}
	}

	public enum TimelineTriggerOffsetKind
	{
		Auto,
		Absolute,
		Relative
	}
}