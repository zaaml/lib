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
		#region Fields

		private readonly DispatcherTimer _timer = new DispatcherTimer();
		private long _currentInvokeCount;

		#endregion

		#region Ctors

		public TimerTrigger()
		{
			_timer.Tick += (sender, args) =>
			{
				Invoke();
				_currentInvokeCount++;
				if (_currentInvokeCount >= InvokeCount)
					IsActive = false;
			};
		}

		#endregion

		#region Properties

		[BindingProxy(PropertyChangedCallback = "t.OnIntervalChanged")]
		public TimeSpan Interval { get; set; }

		[TypeConverter(typeof(LongTypeConverter))]
		[BindingProxy(PropertyChangedCallback = "t.OnInvokeCountChanged")]
		public long InvokeCount { get; set; } = long.MaxValue;

		[BindingProxy(PropertyChangedCallback = "t.OnIsEnabledChanged")]
		public bool IsActive { get; set; } = true;

		#endregion

		#region  Methods

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

		private void OnInvokeCountChanged()
		{
			if (_currentInvokeCount >= InvokeCount)
				IsActive = false;
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
				if (InvokeCount > 0)
					_timer.Start();
			}
			else
			{
				_timer.Stop();
				ResetTimer();
			}
		}

		#endregion
	}

	public class LongTypeConverter : TypeConverter
	{
		#region  Methods

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			long longValue;
			return long.TryParse((string) value, out longValue) ? longValue : 0L;
		}

		#endregion
	}
}