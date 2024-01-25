// <copyright file="LowLevelKeyboardListener.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.Win32.SafeHandles;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak.Collections;
using Zaaml.Platform;

namespace Zaaml.PresentationCore.Input
{
	internal sealed class LowLevelKeyboardListener : IDisposable
	{
		private static readonly WeakLinkedList<LowLevelKeyboardListener> Listeners = new();
		public event LowLevelKeyEventHandler KeyDown;
		public event LowLevelKeyEventHandler KeyUp;

		public LowLevelKeyboardListener()
		{
			Hook ??= new LowLevelKeyboardHook(OnStaticKeyboardCallbackAsync);
			Listeners.Add(this);
		}

		private static LowLevelKeyboardHook Hook { get; set; }

		private void OnKeyboardCallbackAsync(KeyEvent keyEvent, int vkCode)
		{
			switch (keyEvent)
			{
				case KeyEvent.WM_KEYDOWN:
					KeyDown?.Invoke(this, new LowLevelKeyEventArgs(vkCode, false));
					break;
				case KeyEvent.WM_SYSKEYDOWN:
					KeyDown?.Invoke(this, new LowLevelKeyEventArgs(vkCode, true));
					break;
				case KeyEvent.WM_KEYUP:
					KeyUp?.Invoke(this, new LowLevelKeyEventArgs(vkCode, false));
					break;
				case KeyEvent.WM_SYSKEYUP:
					KeyUp?.Invoke(this, new LowLevelKeyEventArgs(vkCode, true));
					break;
			}
		}

		private static void OnStaticKeyboardCallbackAsync(KeyEvent keyEvent, int vkCode)
		{
			if (Listeners.IsEmpty)
			{
				Hook = Hook.DisposeExchange();

				return;
			}

			foreach (var lowLevelKeyboardHook in Listeners)
				lowLevelKeyboardHook.OnKeyboardCallbackAsync(keyEvent, vkCode);
		}

		public void Dispose()
		{
			Listeners.Remove(this);

			if (Listeners.IsEmpty)
				Hook = Hook?.DisposeExchange();
		}

		private delegate void KeyboardCallbackAsync(KeyEvent keyEvent, int vkCode);

		private sealed class LowLevelKeyboardHook : SafeHandleZeroOrMinusOneIsInvalid
		{
			private readonly KeyboardCallbackAsync _hookedKeyboardCallbackAsync;
			private readonly HookProc _hookedLowLevelKeyboardProc;

			public LowLevelKeyboardHook(KeyboardCallbackAsync callback) : base(true)
			{
				_hookedLowLevelKeyboardProc = LowLevelKeyboardProc;
				_hookedKeyboardCallbackAsync = callback;

				using var curProcess = Process.GetCurrentProcess();
				using var curModule = curProcess.MainModule;
				SetHandle(NativeMethods.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, _hookedLowLevelKeyboardProc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0));
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			private IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
			{
				if (nCode < 0)
					return NativeMethods.CallNextHookEx(handle, nCode, wParam, lParam);

				var wParamInt32 = wParam.ToInt32();
				var keyEvent = (KeyEvent) wParamInt32;

				if (keyEvent is not (KeyEvent.WM_KEYDOWN
				    or KeyEvent.WM_KEYUP
				    or KeyEvent.WM_SYSKEYDOWN
				    or KeyEvent.WM_SYSKEYUP))
				{
					return NativeMethods.CallNextHookEx(handle, nCode, wParam, lParam);
				}

				var kbd = (KBDLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
				var vkCode = (int)kbd.vkCode;

				Dispatcher.CurrentDispatcher.BeginInvoke(() => _hookedKeyboardCallbackAsync(keyEvent, vkCode), DispatcherPriority.Input);

				return NativeMethods.CallNextHookEx(handle, nCode, wParam, lParam);
			}

			protected override bool ReleaseHandle()
			{
				return NativeMethods.UnhookWindowsHookEx(handle);
			}
		}

		private enum KeyEvent
		{
			WM_KEYDOWN = 256,
			WM_KEYUP = 257,
			WM_SYSKEYUP = 261,
			WM_SYSKEYDOWN = 260
		}
	}
}