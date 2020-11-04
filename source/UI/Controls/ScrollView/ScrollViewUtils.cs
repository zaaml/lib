// <copyright file="ScrollViewUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;

namespace Zaaml.UI.Controls.ScrollView
{
	internal static class ScrollViewUtils
	{
		public const double DefaultPixelSmallChange = 16.0;
		public const double DefaultPixelWheelChange = 48.0;
		public const double DefaultUnitSmallChange = 1.0;
		public const double DefaultUnitWheelChange = 3.0;

		public static Vector ExecuteScrollCommand(Vector offset, ScrollCommandKind command, Size viewport, Size extent, double smallChange, double wheelChange)
		{
			switch (command)
			{
				case ScrollCommandKind.LineUp:

					offset.Y -= smallChange;

					break;

				case ScrollCommandKind.LineDown:

					offset.Y += smallChange;

					break;

				case ScrollCommandKind.LineLeft:

					offset.X -= smallChange;

					break;

				case ScrollCommandKind.LineRight:

					offset.X += smallChange;

					break;

				case ScrollCommandKind.PageUp:

					offset.Y -= viewport.Height;

					break;

				case ScrollCommandKind.PageDown:

					offset.Y += viewport.Height;

					break;

				case ScrollCommandKind.PageLeft:

					offset.X -= viewport.Width;

					break;

				case ScrollCommandKind.PageRight:

					offset.X += viewport.Width;

					break;

				case ScrollCommandKind.MouseWheelUp:

					offset.Y -= wheelChange;

					break;

				case ScrollCommandKind.MouseWheelDown:

					offset.Y += wheelChange;

					break;

				case ScrollCommandKind.MouseWheelLeft:

					offset.X -= wheelChange;

					break;

				case ScrollCommandKind.MouseWheelRight:

					offset.X += wheelChange;

					break;

				case ScrollCommandKind.ScrollToHome:

					offset = new Vector(0, 0);

					break;

				case ScrollCommandKind.ScrollToBottom:

					offset.Y = extent.Height - viewport.Height;

					break;

				case ScrollCommandKind.ScrollToTop:

					offset.Y = 0;

					break;

				case ScrollCommandKind.ScrollToRight:

					offset.X = extent.Width - viewport.Width;

					break;

				case ScrollCommandKind.ScrollToLeft:

					offset.X = 0;

					break;

				case ScrollCommandKind.ScrollToEnd:

					offset = new Vector(extent.Width - viewport.Width, extent.Height - viewport.Height);

					break;

				default:

					throw new ArgumentOutOfRangeException(nameof(command));
			}

			return offset;
		}

		public static Orientation? GetCommandOrientation(ScrollCommandKind command)
		{
			switch (command)
			{
				case ScrollCommandKind.LineUp:
				case ScrollCommandKind.LineDown:
				case ScrollCommandKind.PageUp:
				case ScrollCommandKind.PageDown:
				case ScrollCommandKind.MouseWheelUp:
				case ScrollCommandKind.MouseWheelDown:
				case ScrollCommandKind.ScrollToTop:
				case ScrollCommandKind.ScrollToBottom:

					return Orientation.Vertical;

				case ScrollCommandKind.LineLeft:
				case ScrollCommandKind.LineRight:
				case ScrollCommandKind.PageLeft:
				case ScrollCommandKind.PageRight:
				case ScrollCommandKind.MouseWheelLeft:
				case ScrollCommandKind.MouseWheelRight:
				case ScrollCommandKind.ScrollToLeft:
				case ScrollCommandKind.ScrollToRight:

					return Orientation.Horizontal;

				case ScrollCommandKind.ScrollToHome:
				case ScrollCommandKind.ScrollToEnd:

					return null;

				default:

					throw new ArgumentOutOfRangeException(nameof(command), command, null);
			}
		}
	}
}