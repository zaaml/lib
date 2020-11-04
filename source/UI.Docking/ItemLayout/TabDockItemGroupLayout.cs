// <copyright file="TabGroupLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  public sealed class TabDockItemGroupLayout : DockItemGroupLayout
  {
    #region Ctors

    public TabDockItemGroupLayout(TabDockItemGroup groupItem)
      : base(groupItem)
    {
    }

    public TabDockItemGroupLayout()
    {
    }

    internal TabDockItemGroupLayout(TabDockItemGroupLayout groupLayout, DockItemLayoutCloneMode mode) : base(groupLayout, mode)
    {
    }

    #endregion

    #region Properties

    internal override DockItemGroupKind GroupKind => DockItemGroupKind.Tab;

    #endregion

    #region  Methods

    internal override DockItemLayout CloneCore(DockItemLayoutCloneMode mode)
    {
      return new TabDockItemGroupLayout(this, mode);
    }

    #endregion
  }
}