// <copyright file="RenderDelayAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore
{
	internal sealed class RenderDelayAction : TriggerDelayActionBase<RenderTrigger, long>
	{
		private readonly Action _action;

		public RenderDelayAction(Action action)
		{
			_action = action;
		}

		public RenderDelayAction(Action action, long delay) : base(delay)
		{
			_action = action;
		}

		protected override bool HasDelay(long delay)
		{
			return delay > 0;
		}

		public void Invoke()
		{
			InvokeCore();
		}

		protected override void OnInvoke()
		{
			_action();
		}

		protected override void OnRevoke()
		{
		}

		protected override void Reset()
		{
		}
	}
}