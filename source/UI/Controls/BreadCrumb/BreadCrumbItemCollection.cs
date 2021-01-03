// <copyright file="BreadCrumbItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.BreadCrumb
{
  public sealed class BreadCrumbItemCollection : DependencyObjectCollectionBase<BreadCrumbItem>
  {
    #region Fields

    private readonly IBreadCrumbItemsOwner _owner;

    #endregion

    #region Ctors

    public BreadCrumbItemCollection()
    {
    }

    internal BreadCrumbItemCollection(IBreadCrumbItemsOwner owner)
    {
      _owner = owner;
    }

    #endregion

    #region  Methods

    protected override void OnItemAdded(BreadCrumbItem item)
    {
      base.OnItemAdded(item);

      item.Owner = _owner;

      _owner.OnItemAdded(item);
    }

    protected override void OnItemRemoved(BreadCrumbItem item)
    {
      _owner.OnItemRemoved(item);

      item.Owner = null;

      base.OnItemRemoved(item);
    }

    #endregion
  }
}