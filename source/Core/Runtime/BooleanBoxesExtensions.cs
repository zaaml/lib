// <copyright file="BooleanBoxesExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Runtime
{
	internal static class BooleanBoxesExtensions
	{
		public static object Box(this bool value)
		{
			return value ? BooleanBoxes.True : BooleanBoxes.False;
		}
	}
}