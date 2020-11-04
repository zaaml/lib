// <copyright file="MouseUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
	internal static class MouseUtils
	{
		public static MouseButtonEventKind FromButtonState(MouseButtonState buttonState)
		{
			return buttonState switch
			{
				MouseButtonState.Released => MouseButtonEventKind.Up,
				MouseButtonState.Pressed => MouseButtonEventKind.Down,
				_ => throw new ArgumentOutOfRangeException(nameof(buttonState), buttonState, null)
			};
		}

		public static MouseButtonKind FromMouseButton(MouseButton mouseButton)
		{
			return mouseButton switch
			{
				MouseButton.Left => MouseButtonKind.Left,
				MouseButton.Middle => MouseButtonKind.Middle,
				MouseButton.Right => MouseButtonKind.Right,
				_ => MouseButtonKind.Undefined
			};
		}
	}
}