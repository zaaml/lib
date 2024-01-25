// <copyright file="DelayStateTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore
{
	public sealed class DelayStateTrigger
	{
		private readonly DelayAction _closeAction;
		private readonly DelayAction _openAction;

		public DelayStateTrigger(Action openAction, TimeSpan openDelay, Action closeAction, TimeSpan closeDelay)
		{
			_openAction = openAction.AsDelayAction(openDelay);
			_closeAction = closeAction.AsDelayAction(closeDelay);
		}

		public DelayStateTrigger(Action openAction, Duration openDelay, Action closeAction, Duration closeDelay)
			: this(openAction, openDelay.HasTimeSpan ? openDelay.TimeSpan : TimeSpan.Zero, closeAction, closeDelay.HasTimeSpan ? closeDelay.TimeSpan : TimeSpan.Zero)
		{
		}

		public TimeSpan CloseDelay
		{
			get => _closeAction.Delay;
			set => _closeAction.Delay = value;
		}

		public TimeSpan OpenDelay
		{
			get => _openAction.Delay;
			set => _openAction.Delay = value;
		}

		public void InvokeClose()
		{
			_closeAction.Invoke();
		}

		public void InvokeOpen()
		{
			_openAction.Invoke();
		}

		public void RevokeClose()
		{
			_closeAction.Revoke();
		}

		public void RevokeOpen()
		{
			_openAction.Revoke();
		}
	}
}