// <copyright file="Band.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.ToolBar
{
	internal class Band
	{
		public readonly List<ToolBarControl> ToolBars;
		public Size DesiredSize;

		public Band()
		{
			ToolBars = new List<ToolBarControl>();
		}

		public Band(IEnumerable<ToolBarControl> toolBars)
		{
			ToolBars = toolBars.ToList();
		}

		internal Rect GetScreenBox()
		{
			return ToolBars.Any()
				? new Rect(ToolBars.First().GetScreenLogicalBox().GetTopLeft(), ToolBars.Last().GetScreenLogicalBox().GetBottomRight())
				: XamlConstants.ZeroRect;
		}
	}
}