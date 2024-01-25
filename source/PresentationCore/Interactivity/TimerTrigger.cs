// <copyright file="TimerTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class TimerTrigger : ActionTriggerBase
	{
		private readonly DispatcherTimer _timer = new();
		private long _currentInvokeCount;
		private bool _isActive = true;
		private long _repeatCount = long.MaxValue;

		public TimerTrigger()
		{
			_timer.Tick += OnTimerOnTick;
		}

		public TimeSpan Interval
		{
			get => _timer.Interval;
			set => _timer.Interval = value;
		}

		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive == value)
					return;

				_isActive = value;

				UpdateTimer();
			}
		}

		[TypeConverter(typeof(LongTypeConverter))]
		public long RepeatCount
		{
			get => _repeatCount;
			set
			{
				if (_repeatCount == value)
					return;

				_repeatCount = value;

				OnRepeatCountChanged();
			}
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var timerTriggerSource = (TimerTrigger)source;

			Interval = timerTriggerSource.Interval;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new TimerTrigger();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			UpdateTimer();
		}

		protected override void OnIsEnabledChanged()
		{
			base.OnIsEnabledChanged();

			UpdateTimer();
		}

		private void OnRepeatCountChanged()
		{
			if (_currentInvokeCount >= RepeatCount)
				IsActive = false;
		}

		private void OnTimerOnTick(object sender, EventArgs args)
		{
			Invoke();

			_currentInvokeCount++;

			if (_currentInvokeCount >= RepeatCount)
				IsActive = false;
		}

		private void ResetTimer()
		{
			_currentInvokeCount = 0;
		}

		public void Start()
		{
			IsActive = true;
		}

		public void Stop()
		{
			IsActive = false;
		}

		private void UpdateTimer()
		{
			if (IsLoaded == false)
				return;

			if (IsEnabled && IsActive)
			{
				if (RepeatCount > 0)
					_timer.Start();
			}
			else
			{
				_timer.Stop();

				ResetTimer();
			}
		}
	}

	public class LongTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return long.TryParse((string)value, out var longValue) ? longValue : 0L;
		}
	}
}