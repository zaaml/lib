// <copyright file="MouseButtonSelectionOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Controls
{
	[Flags]
	internal enum MouseButtonSelectionOptions
	{
		LeftButtonDown = 1,
		RightButtonDown = 2,
		LeftButtonUp = 4,
		RightButtonUp = 8,
	}

	internal static class MouseButtonSelectionHelper
	{
		public static bool ShouldSelect(MouseButtonKind mouseButtonKind, MouseButtonEventKind eventKind, MouseButtonSelectionOptions selectionOptions)
		{
			return (mouseButtonKind, eventKind) switch
			{
				(MouseButtonKind.Left, MouseButtonEventKind.Down) => (selectionOptions & MouseButtonSelectionOptions.LeftButtonDown) != 0,
				(MouseButtonKind.Right, MouseButtonEventKind.Down) => (selectionOptions & MouseButtonSelectionOptions.RightButtonDown) != 0,
				(MouseButtonKind.Left, MouseButtonEventKind.Up) => (selectionOptions & MouseButtonSelectionOptions.LeftButtonUp) != 0,
				(MouseButtonKind.Right, MouseButtonEventKind.Up) => (selectionOptions & MouseButtonSelectionOptions.RightButtonUp) != 0,
				_ => false
			};
		}
	}
}