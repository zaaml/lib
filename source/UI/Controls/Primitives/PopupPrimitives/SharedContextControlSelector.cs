// <copyright file="SharedContextControlSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal class SharedContextControlSelector : SharedItemOwnerCollection
  {
    #region Fields

    private readonly List<ISharedItem> _sharedItems = new List<ISharedItem>();

    #endregion

    #region Ctors

    public SharedContextControlSelector(ISharedItem sharedItem) : base(sharedItem)
    {
    }

    #endregion

    #region  Methods

    protected override void OnCollectionChanged()
    {
      base.OnCollectionChanged();


      foreach (var owner in this)
      {
        foreach (var sharedItem in _sharedItems)
          SharedItemHelper.Share(owner, null, sharedItem);
      }
    }

    public void RegisterSharedItem(ISharedItem sharedItem)
    {
      _sharedItems.Add(sharedItem);

      foreach (var owner in this)
        SharedItemHelper.Share(owner, null, sharedItem);
    }

    public void UnregisterSharedItem(ISharedItem sharedItem)
    {
      foreach (var owner in this)
        SharedItemHelper.Share(owner, sharedItem, null);

      _sharedItems.Remove(sharedItem);
    }

    #endregion
  }
}
