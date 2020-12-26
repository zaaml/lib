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
		private readonly DispatcherTimer _timer = new DispatcherTimer();
		private long _currentInvokeCount;

		public TimerTrigger()
		{
			_timer.Tick += (sender, args) =>
			{
				Invoke();

				_currentInvokeCount++;

				if (_currentInvokeCount >= RepeatCount)
					IsActive = false;
			};
		}

		[BindingProxy(PropertyChangedCallback = "t.OnIntervalChanged")]
		public TimeSpan Interval { get; set; }

		[BindingProxy(PropertyChangedCallback = "t.OnIsActiveChanged")]
		public bool IsActive { get; set; } = true;

		[TypeConverter(typeof(LongTypeConverter))]
		[BindingProxy(PropertyChangedCallback = "t.OnRepeatCountChanged")]
		public long RepeatCount { get; set; } = long.MaxValue;

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var timerTriggerSource = (TimerTrigger) source;

			Interval = timerTriggerSource.Interval;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new TimerTrigger();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			UpdateTimer();

			base.LoadCore(root);
		}

		private void OnIntervalChanged()
		{
			_timer.Interval = Interval;
		}

		private void OnIsActiveChanged()
		{
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
			return long.TryParse((string) value, out var longValue) ? longValue : 0L;
		}
	}
}