// <copyright file="DelayAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Threading;
using Zaaml.Core.Pools;

namespace Zaaml.PresentationCore
{
	public abstract class DelayActionBase
	{
		private readonly Implementation _implementation;

		protected DelayActionBase()
		{
			_implementation = new Implementation(this);
		}

		protected DelayActionBase(TimeSpan delay) : this()
		{
			Delay = delay;
		}

		public TimeSpan Delay
		{
			get => _implementation.Delay;
			set => _implementation.Delay = value;
		}

		public bool InvokeQueried => _implementation.InvokeQueried;

		public int RepeatCount
		{
			get => _implementation.RepeatCount;
			set => _implementation.RepeatCount = value;
		}

		internal void ForceDelayComplete()
		{
			_implementation.ForceDelayComplete();
		}

		protected void InvokeCore()
		{
			_implementation.Invoke();
		}

		protected void InvokeCore(TimeSpan delay)
		{
			_implementation.Invoke(delay);
		}

		protected abstract void OnInvoke();

		protected abstract void OnRevoke();

		protected abstract void Reset();

		public void Revoke()
		{
			_implementation.Revoke();
		}

		private sealed class Implementation
		{
			private static readonly ObjectPool<DispatcherTimer> TimerPool = new ObjectPool<DispatcherTimer>(CreateTimer, InitializeTimer, CleanTimer, int.MaxValue);
			private readonly DelayActionBase _delayActionBase;
			private int _currentInvokeCount;
			private TimeSpan _delay;

			public Implementation(DelayActionBase delayActionBase)
			{
				_delayActionBase = delayActionBase;
			}

			public TimeSpan Delay
			{
				get => _delay;
				set
				{
					_delay = value;

					if (Timer != null)
						Timer.Interval = value;
				}
			}

			public bool InvokeQueried { get; private set; }

			public int RepeatCount { get; set; }

			private DispatcherTimer Timer { get; set; }

			private static void CleanTimer(DispatcherTimer timer)
			{
				timer.Stop();
			}

			public static DelayAction Create(Action action, TimeSpan delay)
			{
				return new DelayAction(action, delay);
			}

			private static DispatcherTimer CreateTimer()
			{
				return new DispatcherTimer();
			}

			private static void InitializeTimer(DispatcherTimer obj)
			{
			}

			public void Invoke()
			{
				InvokeImpl(Delay);
			}

			public void Invoke(TimeSpan delay)
			{
				InvokeImpl(delay);
			}

			private void InvokeImpl(TimeSpan delay)
			{
				_currentInvokeCount = 0;

				if (delay.Equals(TimeSpan.Zero))
				{
					_delayActionBase.OnInvoke();

					return;
				}

				InvokeQueried = true;

				if (Timer != null)
				{
					Timer.Stop();
					Timer.Interval = delay;
					Timer.Start();

					return;
				}

				MountTimer(delay);
			}

			private void MountTimer(TimeSpan delay)
			{
				Timer = TimerPool.GetObject();
				Timer.Interval = delay;
				Timer.Tick += TimerOnTick;
				Timer.Start();
			}

			private void ReleaseTimer()
			{
				if (Timer == null)
					return;

				Timer.Stop();
				Timer.Tick -= TimerOnTick;
				TimerPool.Release(Timer);

				Timer = null;
			}

			public void Revoke()
			{
				ReleaseTimer();

				_delayActionBase.OnRevoke();
				_delayActionBase.Reset();
			}

			private void TimerOnTick(object sender, EventArgs eventArgs)
			{
				OnTimer();
			}

			private void OnTimer()
			{
				_delayActionBase.OnInvoke();

				_currentInvokeCount++;

				if (_currentInvokeCount >= RepeatCount)
				{
					ReleaseTimer();

					InvokeQueried = false;

					_delayActionBase.Reset();
				}
			}

			public void ForceDelayComplete()
			{
				if (Timer == null)
					return;

				OnTimer();
			}
		}
	}

	public sealed partial class DelayAction : DelayActionBase
	{
		public DelayAction(Action action, TimeSpan delay) : base(delay)
		{
			Action = action;
		}

		public DelayAction(Action action)
		{
			Action = action;
		}

		public Action Action { get; }

		public void Invoke()
		{
			InvokeCore();
		}

		public void Invoke(TimeSpan delay)
		{
			InvokeCore(delay);
		}

		protected override void OnInvoke()
		{
			Action();
		}

		protected override void OnRevoke()
		{
		}

		protected override void Reset()
		{
		}

		public static DelayAction StaticInvoke(Action action, TimeSpan delay)
		{
			if (delay.Equals(TimeSpan.Zero))
			{
				action();

				return null;
			}

			var delayAction = new DelayAction(action, delay);

			delayAction.Invoke();

			return delayAction;
		}

		public static Action Wrap(Action action, TimeSpan delay)
		{
			if (delay.Equals(TimeSpan.Zero))
				return action;

			var delayAction = new DelayAction(action, delay);

			return () => delayAction.Invoke();
		}
	}

	public static partial class DelayActionExtensions
	{
		public static DelayAction AsDelayAction(this Action action)
		{
			return new DelayAction(action);
		}

		public static DelayAction AsDelayAction(this Action action, TimeSpan delay)
		{
			return new DelayAction(action, delay);
		}

		public static DelayAction DelayInvoke(this Action action, TimeSpan delay)
		{
			return DelayAction.StaticInvoke(action, delay);
		}

		public static DelayAction RevokeExchange(this DelayAction delayAction, DelayAction newDelayAction = null)
		{
			delayAction?.Revoke();

			return newDelayAction;
		}
	}
}