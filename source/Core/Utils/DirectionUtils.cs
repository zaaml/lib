// <copyright file="DirectionUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Utils
{
	internal static class DirectionUtils
	{
		public static Direction Invert(Direction direction)
		{
			return direction == Direction.Forward ? Direction.Backward : Direction.Forward;
		}

		public static Direction WithInvert(Direction direction, bool invert)
		{
			return invert == false ? direction : Invert(direction);
		}
	}
}