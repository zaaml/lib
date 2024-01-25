namespace Zaaml.Core
{
	internal interface ISuspendState
	{
		bool IsSuspended { get; }

		void Resume();

		void Suspend();
	}
}