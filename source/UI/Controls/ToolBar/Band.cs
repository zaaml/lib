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
    #region Fields

    public readonly List<ToolBarControl> ToolBars;
    public Size DesiredSize;

    #endregion

    #region Ctors

    public Band()
    {
      ToolBars = new List<ToolBarControl>();
    }

    public Band(IEnumerable<ToolBarControl> toolBars)
    {
      ToolBars = toolBars.ToList();
    }

    #endregion

    #region  Methods

    internal Rect GetHostBox()
    {
      return ToolBars.Any()
        ? new Rect(ToolBars.First().GetScreenBox().GetTopLeft(), ToolBars.Last().GetScreenBox().GetBottomRight())
        : XamlConstants.ZeroRect;
    }

    #endregion
  }
}