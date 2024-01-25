using System;

namespace Zaaml.Core
{
	internal sealed class DelegateObservableSuspendState : SuspendStateBase
	{
		private readonly Action _onResume;
		private readonly Action _onSuspend;

		public DelegateObservableSuspendState(Action onSuspend, Action onResume)
		{
			_onSuspend = onSuspend;
			_onResume = onResume;
		}

		protected override void OnResume()
		{
			_onResume();
		}

		protected override void OnSuspend()
		{
			_onSuspend();
		}
	}
}