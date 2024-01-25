using System;
using Zaaml.Core.Disposable;

namespace Zaaml.Core
{
	internal static class Suspender
	{
		public static IDisposable EnterSuspendState(this ISuspendState suspendState)
		{
			return new DelegateDisposable(suspendState.Suspend, suspendState.Resume);
		}
	}
}