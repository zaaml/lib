// <copyright file="DefaultTabViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
  internal class DefaultTabViewItemGenerator : TabViewItemGeneratorBase, IDelegatedGenerator<TabViewItem>
  {
    #region  Methods

    protected override void AttachItem(TabViewItem item, object itemSource)
    {
      Implementation.AttachItem(item, itemSource);
    }

    protected override TabViewItem CreateItem(object itemSource)
    {
      return Implementation.CreateItem(itemSource);
    }

    protected override void DetachItem(TabViewItem item, object itemSource)
    {
      Implementation.DetachItem(item, itemSource);
    }

    protected override void DisposeItem(TabViewItem item, object itemSource)
    {
      Implementation.DisposeItem(item, itemSource);
    }

    #endregion

    #region Interface Implementations

    #region IDelegatedGenerator<TabViewItem>

    public IItemGenerator<TabViewItem> Implementation { get; set; }

    #endregion

    #endregion
  }
}