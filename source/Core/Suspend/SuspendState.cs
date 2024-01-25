// <copyright file="SuspendState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal class SuspendState : ISuspendState
	{
		private int _suspendCount;

		public bool IsSuspended => _suspendCount > 0;

		public void Resume()
		{
			if (_suspendCount > 0)
				_suspendCount--;
		}

		public void Suspend()
		{
			_suspendCount++;
		}
	}
}