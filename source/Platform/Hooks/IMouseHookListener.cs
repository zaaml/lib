// <copyright file="IMouseHookListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Platform.Hooks
{
	internal interface IMouseHookListener
	{
		void OnMouse(IntPtr hwnd, int msg, POINT point);
	}
}