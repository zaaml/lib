// <copyright file="CallWndProcHookService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Zaaml.Platform.Hooks
{
	internal sealed class CallWndProcHookService : WindowHookServiceBase<ICallWndProcHookListener>
	{
		#region Static Fields and Constants

		private static readonly Lazy<CallWndProcHookService> LazyInstance = new Lazy<CallWndProcHookService>(() => new CallWndProcHookService());

		#endregion

		#region Ctors

		private CallWndProcHookService() : base(HookType.WH_CALLWNDPROC)
		{
		}

		#endregion

		#region Properties

		public static CallWndProcHookService Instance => LazyInstance.Value;

		#endregion

		#region  Methods

		protected override IntPtr OnHookProc(int code, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var msg = (CWPSTRUCT) Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));

			try
			{
				LockListeners();

				foreach (var listener in Listeners)
					listener.OnCallWndProc(msg.hwnd, msg.message, msg.wparam, msg.lparam);
			}
			finally
			{
				UnlockListeners();
			}

			return IntPtr.Zero;
		}

		#endregion
	}
}