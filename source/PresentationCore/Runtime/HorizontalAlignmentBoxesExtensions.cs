using System;
using System.Windows;

namespace Zaaml.PresentationCore.Runtime
{
	internal static class HorizontalAlignmentBoxesExtensions
	{
		public static object Box(this HorizontalAlignment value)
		{
			return value switch
			{
				HorizontalAlignment.Left => HorizontalAlignmentBoxes.Left,
				HorizontalAlignment.Center => HorizontalAlignmentBoxes.Center,
				HorizontalAlignment.Right => HorizontalAlignmentBoxes.Right,
				HorizontalAlignment.Stretch => HorizontalAlignmentBoxes.Stretch,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}