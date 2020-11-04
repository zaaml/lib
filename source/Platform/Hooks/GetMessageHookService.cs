// <copyright file="GetMessageHookService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Zaaml.Platform.Hooks
{
	internal sealed class GetMessageHookService : WindowHookServiceBase<IGetMessageHookListener>
	{
		#region Static Fields and Constants

		private static readonly Lazy<GetMessageHookService> LazyInstance = new Lazy<GetMessageHookService>(() => new GetMessageHookService());

		#endregion

		#region Ctors

		private GetMessageHookService() : base(HookType.WH_GETMESSAGE)
		{
		}

		#endregion

		#region Properties

		public static GetMessageHookService Instance => LazyInstance.Value;

		#endregion

		#region  Methods

		protected override IntPtr OnHookProc(int code, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var msg = (Msg)Marshal.PtrToStructure(lParam, typeof(Msg));

			foreach (var listener in Listeners)
				listener.OnGetMessage(msg.Hwnd, msg.Message, msg.wParam, msg.lParam);

			return IntPtr.Zero;
		}

		#endregion
	}
}