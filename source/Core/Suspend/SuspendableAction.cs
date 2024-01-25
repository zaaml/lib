// <copyright file="SuspendableAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
	internal static class SuspendableAction
	{
		public static Action Create(Action action, ObservableSuspendState suspendState)
		{
			var suspendableAction = new SuspendableActionImpl(action, suspendState);

			return suspendableAction.Invoke;
		}

		private class SuspendableActionImpl
		{
			private readonly Action _action;
			private readonly ObservableSuspendState _suspendState;
			private bool _invokeQueried;

			public SuspendableActionImpl(Action action, ObservableSuspendState suspendState)
			{
				_suspendState = suspendState;
				_action = action;

				suspendState.Resumed += SuspendStateOnResumed;
			}


			public void Invoke()
			{
				if (_suspendState.IsSuspended)
					_invokeQueried = true;
				else
					_action();
			}

			private void SuspendStateOnResumed(object sender, EventArgs eventArgs)
			{
				if (_invokeQueried == false)
					return;

				_invokeQueried = false;
				_action();
			}
		}
	}
}