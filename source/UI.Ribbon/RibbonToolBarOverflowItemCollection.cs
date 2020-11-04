// <copyright file="RibbonToolBarOverflowItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Primitives.Overflow;

namespace Zaaml.UI.Controls.Ribbon
{
  internal sealed class RibbonToolBarOverflowItemCollection : OverflowItemCollection<RibbonToolBarOverflowItemsPresenter, RibbonItem>
  {
    #region Ctors

    public RibbonToolBarOverflowItemCollection(RibbonToolBarOverflowItemsPresenter overflowItemsPresenter) : base(overflowItemsPresenter)
    {
    }

    #endregion

    #region  Methods

    protected override void OnItemAttached(OverflowItem<RibbonItem> item)
    {
      Control.OnItemAttached(item);

      base.OnItemAttached(item);
    }

    protected override void OnItemDetached(OverflowItem<RibbonItem> item)
    {
      base.OnItemDetached(item);

      Control.OnItemDetached(item);
    }

    #endregion
  }
}