using System;
using System.Windows;

namespace Zaaml.PresentationCore.Runtime
{
	internal static class VerticalAlignmentBoxesExtensions
	{
		public static object Box(this VerticalAlignment value)
		{
			return value switch
			{
				VerticalAlignment.Top => VerticalAlignmentBoxes.Top,
				VerticalAlignment.Center => VerticalAlignmentBoxes.Center,
				VerticalAlignment.Bottom => VerticalAlignmentBoxes.Bottom,
				VerticalAlignment.Stretch => VerticalAlignmentBoxes.Stretch,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}