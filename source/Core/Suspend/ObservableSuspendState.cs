using System;

namespace Zaaml.Core
{
	internal sealed class ObservableSuspendState : SuspendStateBase
	{
		public event EventHandler Resumed;
		public event EventHandler Suspended;

		protected override void OnResume()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnSuspend()
		{
			Suspended?.Invoke(this, EventArgs.Empty);
		}
	}
}