// <copyright file="DockItemLayoutExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  internal static class DockItemLayoutExtensions
  {
    #region  Methods

    internal static bool IsSimple(this DockItemLayout dockItemLayout)
    {
      return dockItemLayout is DockItemGroupLayout == false;
    }

    #endregion
  }
}