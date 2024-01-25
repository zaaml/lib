namespace Zaaml.Core
{
	internal class DummySuspendState : ISuspendState
	{
		public static readonly ISuspendState Instance = new DummySuspendState();

		private DummySuspendState()
		{
		}

		public bool IsSuspended => false;

		public void Resume()
		{
		}

		public void Suspend()
		{
		}
	}
}