// <copyright file="GridViewCellMode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core.GridView
{
	public enum GridViewCellMode
	{
		Display,
		Edit
	}

	internal static class GridViewCellModeBoxes
	{
		internal static readonly object Display = GridViewCellMode.Display;
		internal static readonly object Edit = GridViewCellMode.Edit;
	}

	internal static class GridViewCellModeBoxesExtensions
	{
		public static object Box(this GridViewCellMode value)
		{
			return value switch
			{
				GridViewCellMode.Display => GridViewCellModeBoxes.Display,
				GridViewCellMode.Edit => GridViewCellModeBoxes.Edit,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}