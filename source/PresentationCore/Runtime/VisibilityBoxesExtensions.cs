using System;
using System.Windows;

namespace Zaaml.PresentationCore.Runtime
{
	internal static class VisibilityBoxesExtensions
	{
		public static object Box(this Visibility value)
		{
			return value switch
			{
				Visibility.Visible => VisibilityBoxes.Visible,
				Visibility.Hidden => VisibilityBoxes.Hidden,
				Visibility.Collapsed => VisibilityBoxes.Collapsed,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}