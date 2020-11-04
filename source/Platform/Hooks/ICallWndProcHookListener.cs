// <copyright file="ICallWndProcHookListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Platform.Hooks
{
	internal interface ICallWndProcHookListener
	{
		void OnCallWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
	}
}