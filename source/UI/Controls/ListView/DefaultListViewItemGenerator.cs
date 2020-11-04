// <copyright file="DefaultListViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
  internal class DefaultListViewItemGenerator : ListViewItemGeneratorBase, IDelegatedGenerator<ListViewItem>
  {
    #region  Methods

    protected override void AttachItem(ListViewItem item, object itemSource)
    {
      Implementation.AttachItem(item, itemSource);
    }

    protected override ListViewItem CreateItem(object itemSource)
    {
      return Implementation.CreateItem(itemSource);
    }

    protected override void DetachItem(ListViewItem item, object itemSource)
    {
      Implementation.DetachItem(item, itemSource);
    }

    protected override void DisposeItem(ListViewItem item, object itemSource)
    {
      Implementation.DisposeItem(item, itemSource);
    }

    #endregion

    #region Interface Implementations

    #region IDelegatedGenerator<ListViewItem>

    public IItemGenerator<ListViewItem> Implementation { get; set; }

    #endregion

    #endregion
  }
}
