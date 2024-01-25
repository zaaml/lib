using System;

namespace Zaaml.PresentationCore.Runtime
{
	internal static class ElementVisibilityBoxesExtensions
	{
		public static object Box(this ElementVisibility value)
		{
			return value switch
			{
				ElementVisibility.Auto => ElementVisibilityBoxes.Auto,
				ElementVisibility.Inherit => ElementVisibilityBoxes.Inherit,
				ElementVisibility.Collapsed => ElementVisibilityBoxes.Collapsed,
				ElementVisibility.Hidden => ElementVisibilityBoxes.Hidden,
				ElementVisibility.Visible => ElementVisibilityBoxes.Visible,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}