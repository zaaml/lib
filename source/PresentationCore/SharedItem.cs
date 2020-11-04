// <copyright file="SharedItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.PresentationCore
{
  internal interface ISharedItem
  {
    #region Properties

    bool IsShared { get; set; }

    SharedItemOwnerCollection Owners { get; }

    #endregion
  }

  internal class SharedItemOwnerCollection : WeakLinkedList<FrameworkElement>
  {
    #region Ctors

    public SharedItemOwnerCollection(ISharedItem sharedItem)
    {
      SharedItem = sharedItem;
    }

    #endregion

    #region Properties

    public ISharedItem SharedItem { get; }

    #endregion

    #region  Methods

    protected override void OnCollectionChanged()
    {
      base.OnCollectionChanged();

      SharedItem.IsShared = IsEmpty == false;
    }

    #endregion
  }

  internal static class SharedItemHelper
  {
    #region  Methods

    public static void Share<T>(FrameworkElement owner, T oldSharedItem, T newSharedItem) where T : ISharedItem
    {
      oldSharedItem?.Owners.Remove(owner);
      newSharedItem?.Owners.Add(owner);
    }

    #endregion
  }
}
