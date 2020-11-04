// <copyright file="RibbonItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonItemCollection : ItemCollectionBase<Control, RibbonItem>
  {
    #region Ctors

    internal RibbonItemCollection(RibbonGroup group) : base(group)
    {
      Group = group;
    }

    internal RibbonItemCollection(RibbonToolBar toolBar) : base(toolBar)
    {
      ToolBar = toolBar;
    }

    #endregion

    #region Properties

    protected override ItemGenerator<RibbonItem> DefaultGenerator => throw new NotImplementedException();

    internal RibbonGroup Group { get; }

    internal RibbonToolBar ToolBar { get; }

    #endregion
  }
}