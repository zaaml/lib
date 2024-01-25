// <copyright file="RawKeyEventArgs.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
	internal sealed class LowLevelKeyEventArgs : EventArgs
	{
		public LowLevelKeyEventArgs(int virtualKey, bool isSysKey)
		{
			VirtualKey = virtualKey;
			IsSysKey = isSysKey;
			Key = KeyInterop.KeyFromVirtualKey(virtualKey);
		}

		public bool IsSysKey { get; }

		public Key Key { get; }

		public int VirtualKey { get; }
	}
}