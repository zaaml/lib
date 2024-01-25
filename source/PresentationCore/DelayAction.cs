// <copyright file="DelayAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore
{
	public abstract class DelayActionBase : TriggerDelayActionBase<DelayTimerTrigger, TimeSpan>
	{
		protected DelayActionBase()
		{
		}

		protected DelayActionBase(TimeSpan delay) : base(delay)
		{
		}

		protected override bool HasDelay(TimeSpan delay)
		{
			return delay.Equals(TimeSpan.Zero) == false;
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
}