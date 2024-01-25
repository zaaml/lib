using System;

namespace Zaaml.PresentationCore
{
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