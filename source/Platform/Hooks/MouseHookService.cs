// <copyright file="MouseHookService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Zaaml.Platform.Hooks
{
	internal sealed class MouseHookService : WindowHookServiceBase<IMouseHookListener>
	{
		#region Static Fields and Constants

		private static readonly Lazy<MouseHookService> LazyInstance = new Lazy<MouseHookService>(() => new MouseHookService());

		#endregion

		#region Ctors

		private MouseHookService() : base(HookType.WH_MOUSE)
		{
		}

		#endregion

		#region Properties

		public static MouseHookService Instance => LazyInstance.Value;

		#endregion

		#region  Methods

		protected override IntPtr OnHookProc(int code, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var msg = (MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MOUSEHOOKSTRUCT));

			foreach (var listener in Listeners)
				listener.OnMouse(msg.hwnd, wParam.ToInt32(), msg.pt);

			return IntPtr.Zero;
		}

		#endregion
	}
}