// <copyright file="MouseAutomation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows;
using Zaaml.Platform;
using Zaaml.PresentationCore.Animation;
using Zaaml.PresentationCore.Input;

namespace Zaaml.PresentationCore.Automation
{
	internal sealed class MouseAutomation : InputAutomation
	{
		public MouseAutomation()
		{
		}

		public MouseAutomation(MouseAutomationOptions options)
		{
			Options = options;
		}

		public MouseAutomationOptions Options { get; }

		protected override InputAutomationOptions OptionsCore => Options;

		private static Point GetCursorPosition()
		{
			return MouseInternal.ScreenLogicalPosition;
		}

		public async Task MouseDownAsync(MouseButtonKind button)
		{
			await PreEventDelay();

			var mouseEvent = button switch
			{
				MouseButtonKind.Left => MOUSEEVENT.LEFTDOWN,
				MouseButtonKind.Right => MOUSEEVENT.RIGHTDOWN,
				MouseButtonKind.Middle => MOUSEEVENT.MIDDLEDOWN,
				_ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
			};

			NativeMethods.mouse_event(mouseEvent, 0, 0, 0, 0);

			await PostEventDelay();
		}

		public async Task MouseUpAsync(MouseButtonKind button)
		{
			await PreEventDelay();

			var mouseEvent = button switch
			{
				MouseButtonKind.Left => MOUSEEVENT.LEFTUP,
				MouseButtonKind.Right => MOUSEEVENT.RIGHTUP,
				MouseButtonKind.Middle => MOUSEEVENT.MIDDLEUP,
				_ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
			};

			NativeMethods.mouse_event(mouseEvent, 0, 0, 0, 0);

			await PostEventDelay();
		}

		public async Task MoveAsync(Point position)
		{
			await PreEventDelay();

			await AnimationTask.RunAsync(GetCursorPosition(), position, Options.MoveDuration, SetCursorPosition);

			await PostEventDelay();
		}

		private static void SetCursorPosition(Point position)
		{
			MouseInternal.ScreenLogicalPosition = position;
		}
	}
}