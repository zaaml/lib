using System;
using Zaaml.Core.Pools;

namespace Zaaml.PresentationCore
{
	public abstract class TriggerDelayActionBase<TDelayTrigger, TDelay> where TDelayTrigger : DelayTriggerBase<TDelay>, new()
	{
		private readonly Implementation _implementation;

		protected TriggerDelayActionBase()
		{
			_implementation = new Implementation(this);
		}

		protected TriggerDelayActionBase(TDelay delay) : this()
		{
			Delay = delay;
		}

		public TDelay Delay
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

		protected void InvokeCore(TDelay delay)
		{
			_implementation.Invoke(delay);
		}

		protected abstract bool HasDelay(TDelay delay);

		protected abstract void OnInvoke();

		protected abstract void OnRevoke();

		protected abstract void Reset();

		public void Revoke()
		{
			_implementation.Revoke();
		}

		private sealed class Implementation
		{
			private static readonly ObjectPool<TDelayTrigger> TriggerPool = new(CreateTrigger, InitializeTrigger, CleanTrigger, int.MaxValue);
			private readonly TriggerDelayActionBase<TDelayTrigger, TDelay> _delayActionBase;
			private int _currentInvokeCount;
			private TDelay _delay;

			public Implementation(TriggerDelayActionBase<TDelayTrigger, TDelay> delayActionBase)
			{
				_delayActionBase = delayActionBase;
			}

			public TDelay Delay
			{
				get => _delay;
				set
				{
					_delay = value;

					if (Trigger != null)
						Trigger.Delay = value;
				}
			}

			public bool InvokeQueried { get; private set; }

			public int RepeatCount { get; set; }

			private TDelayTrigger Trigger { get; set; }

			private static void CleanTrigger(TDelayTrigger trigger)
			{
				trigger.Stop();
			}

			private static TDelayTrigger CreateTrigger()
			{
				return new TDelayTrigger();
			}

			private static void InitializeTrigger(TDelayTrigger trigger)
			{
			}

			public void Invoke()
			{
				InvokeImpl(Delay);
			}

			public void Invoke(TDelay delay)
			{
				InvokeImpl(delay);
			}

			private void InvokeImpl(TDelay delay)
			{
				_currentInvokeCount = 0;

				if (_delayActionBase.HasDelay(delay) == false)
				{
					_delayActionBase.OnInvoke();

					return;
				}

				InvokeQueried = true;

				if (Trigger != null)
				{
					Trigger.Stop();
					Trigger.Delay = delay;
					Trigger.Start();

					return;
				}

				MountTrigger(delay);
			}

			private void MountTrigger(TDelay delay)
			{
				Trigger = TriggerPool.GetObject();
				Trigger.Delay = delay;
				Trigger.Trigger += TriggerOnTick;
				Trigger.Start();
			}

			private void ReleaseTrigger()
			{
				if (Trigger == null)
					return;

				Trigger.Stop();
				Trigger.Trigger -= TriggerOnTick;
				TriggerPool.Release(Trigger);

				Trigger = null;
			}

			public void Revoke()
			{
				ReleaseTrigger();

				_delayActionBase.OnRevoke();
				_delayActionBase.Reset();
			}

			private void TriggerOnTick(object sender, EventArgs eventArgs)
			{
				OnTrigger();
			}

			private void OnTrigger()
			{
				_delayActionBase.OnInvoke();

				_currentInvokeCount++;

				if (_currentInvokeCount >= RepeatCount)
				{
					ReleaseTrigger();

					InvokeQueried = false;

					_delayActionBase.Reset();
				}
			}

			public void ForceDelayComplete()
			{
				if (Trigger == null)
					return;

				OnTrigger();
			}
		}
	}
}