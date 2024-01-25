namespace Zaaml.Core
{
	internal abstract class SuspendStateBase : ISuspendState
	{
		protected int SuspendCount { get; private set; }

		protected abstract void OnResume();

		protected abstract void OnSuspend();

		public bool IsSuspended => SuspendCount > 0;

		public void Resume()
		{
			if (SuspendCount == 0)
				return;

			SuspendCount--;

			if (SuspendCount == 0)
				OnResume();
		}

		public void Suspend()
		{
			if (SuspendCount == 0)
				OnSuspend();

			SuspendCount++;
		}
	}
}