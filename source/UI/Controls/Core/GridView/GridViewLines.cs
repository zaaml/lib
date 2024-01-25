// <copyright file="GridViewLines.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core.GridView
{
	[Flags]
	public enum GridViewLines
	{
		None = 0,
		Both = Vertical | Horizontal,
		Vertical = 1,
		Horizontal = 2,
	}

	public static class GridViewLinesExtensions
	{
		public static bool ShowHorizontal(this GridViewLines gridViewLines)
		{
			return (gridViewLines & GridViewLines.Horizontal) != 0;
		}

		public static bool ShowVertical(this GridViewLines gridViewLines)
		{
			return (gridViewLines & GridViewLines.Vertical) != 0;
		}
	}
}