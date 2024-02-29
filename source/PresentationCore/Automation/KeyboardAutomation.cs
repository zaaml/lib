// <copyright file="KeyboardAutomation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using System.Windows.Input;
using Zaaml.Platform;

namespace Zaaml.PresentationCore.Automation
{
	internal sealed class KeyboardAutomation : InputAutomation
	{
		public KeyboardAutomation()
		{
		}

		public KeyboardAutomation(KeyboardAutomationOptions options)
		{
			Options = options;
		}

		public KeyboardAutomationOptions Options { get; }

		protected override InputAutomationOptions OptionsCore => Options;

		public async Task PressAsync(Key key)
		{
			await PreEventDelay();

			var vk = (byte)KeyInterop.VirtualKeyFromKey(key);

			NativeMethods.keybd_event(vk, 0, 0, 0);
			NativeMethods.keybd_event(vk, 0, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP, 0);

			await PostEventDelay();
		}
	}
}