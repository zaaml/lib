// <copyright file="DelayRenderActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Zaaml.PresentationCore
{
	public abstract class DelayTriggerBase<TDelay>
	{
		public abstract event EventHandler Trigger;

		public abstract TDelay Delay { get; set; }

		public abstract void Start();

		public abstract void Stop();
	}

	public sealed class DelayTimerTrigger : DelayTriggerBase<TimeSpan>
	{
		private readonly DispatcherTimer _timer;

		public override event EventHandler Trigger;

		public DelayTimerTrigger()
		{
			_timer = new DispatcherTimer();
			_timer.Tick += TimerOnTick;
		}

		public override TimeSpan Delay
		{
			get => _timer.Interval;
			set => _timer.Interval = value;
		}

		public override void Start()
		{
			_timer.Start();
		}

		public override void Stop()
		{
			_timer.Stop();
		}

		private void TimerOnTick(object sender, EventArgs e)
		{
			Trigger?.Invoke(this, EventArgs.Empty);
		}
	}

	public sealed class RenderTrigger : DelayTriggerBase<long>
	{
		private long _delay;
		private long _frame;

		public override event EventHandler Trigger;

		public override long Delay
		{
			get => _delay;
			set
			{
				_delay = value;
				_frame = 0;
			}
		}

		private void CompositionTargetOnRendering(object sender, EventArgs e)
		{
			if (++_frame == Delay)
			{
				Trigger?.Invoke(this, EventArgs.Empty);
			}
		}

		public override void Start()
		{
			_frame = 0;

			CompositionTarget.Rendering += CompositionTargetOnRendering;
		}

		public override void Stop()
		{
			CompositionTarget.Rendering -= CompositionTargetOnRendering;
		}
	}
}