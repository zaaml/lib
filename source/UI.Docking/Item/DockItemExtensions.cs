// <copyright file="DockItemExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  internal static class DockItemExtensions
  {
    #region  Methods

    public static DockItemGroup AutoHideContainer(this DockItem item)
    {
      return item.GetParentGroup(DockItemState.AutoHide);
    }

    public static DockItemGroup DockContainer(this DockItem item)
    {
      return item.GetParentGroup(DockItemState.Dock);
    }

    public static DockItemGroup DocumentContainer(this DockItem item)
    {
      return item.GetParentGroup(DockItemState.Document);
    }

    public static DockItemGroup FloatContainer(this DockItem item)
    {
      return item.GetParentGroup(DockItemState.Float);
    }

    public static DockItemGroup HiddenContainer(this DockItem item)
    {
      return item.GetParentGroup(DockItemState.Hidden);
    }

    public static bool IsAutoHide(this DockItem item)
    {
      return item.DockState == DockItemState.AutoHide;
    }

    public static bool IsContainer(this DockItem item)
    {
      return item is DockItemGroup;
    }

    public static bool IsDock(this DockItem item)
    {
      return item.DockState == DockItemState.Dock;
    }

    public static bool IsDocument(this DockItem item)
    {
      return item.DockState == DockItemState.Document;
    }

    public static bool IsFloat(this DockItem item)
    {
      return item.DockState == DockItemState.Float;
    }

    public static bool IsHidden(this DockItem item)
    {
      return item.DockState == DockItemState.Hidden;
    }

    public static bool IsSimple(this DockItem item)
    {
      return item is DockItemGroup == false;
    }

    #endregion
  }
}