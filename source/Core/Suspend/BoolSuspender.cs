using System;
using Zaaml.Core.Disposable;

namespace Zaaml.Core
{
	internal class BoolSuspender
	{
		public bool IsSuspended { get; private set; }

		public void Resume()
		{
			IsSuspended = false;
		}

		public void Suspend()
		{
			IsSuspended = true;
		}

		public static IDisposable Suspend(BoolSuspender suspender)
		{
			return DelegateDisposable.Create(suspender.Suspend, suspender.Resume);
		}
	}
}