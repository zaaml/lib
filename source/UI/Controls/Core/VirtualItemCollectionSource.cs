// <copyright file="VirtualItemCollectionSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
  internal sealed class VirtualItemCollectionSource<TControl, T> : ItemCollectionSourceBase<TControl, T>
	  where T : System.Windows.Controls.Control
		where TControl : System.Windows.Controls.Control
  {
    #region Fields

    private IEnumerable _source;

    #endregion

    #region Ctors

    public VirtualItemCollectionSource(IVirtualItemsHost<T> itemsHost, ItemCollectionBase<TControl, T> itemCollection) : base(itemsHost, itemCollection)
    {
      VirtualCollection = new VirtualItemCollection(itemCollection);

      itemsHost.VirtualSource = VirtualCollection;
    }

    #endregion

    #region Properties

    private VirtualItemCollection<TControl, T> VirtualCollection { get; }

    public override IEnumerable<T> ActualItems => VirtualCollection.ActualItems;

    public override int Count => VirtualCollection.Count;

    public override IEnumerable Source
    {
      get => _source;
      set
      {
        if (ReferenceEquals(_source, value))
          return;

        _source = value;

        VirtualCollection.Source = value;

        OnInvalidated();
      }
    }

    #endregion

    #region  Methods

    protected override void OnGeneratorChanged()
    {
      base.OnGeneratorChanged();

      VirtualCollection.Generator = Generator;
    }

    private void OnInvalidated()
    {
      (ItemsHost as Panel)?.InvalidateMeasure();
    }

    public override T EnsureItem(int index)
    {
      return VirtualCollection.EnsureItem(index);
    }

    protected override int GetIndexFromItem(T item)
    {
      return VirtualCollection.GetIndexFromItem(item);
    }

    protected override int GetIndexFromItemSource(object itemSource)
    {
      return VirtualCollection.GetIndexFromItemSource(itemSource);
    }

    protected override T GetItemFromIndex(int index)
    {
      return VirtualCollection.GetItemFromIndex(index);
    }

    protected override object GetItemSourceFromIndex(int index)
    {
      return VirtualCollection.GetItemSourceFromIndex(index);
    }

    public override void LockItem(T item)
    {
      VirtualCollection.LockItem(item);
    }

    public override void UnlockItem(T item)
    {
      VirtualCollection.UnlockItem(item);
    }

    public override void Dispose()
    {
      base.Dispose();

      ((IVirtualItemsHost<T>) ItemsHost).VirtualSource = null;
    }

    #endregion

    #region  Nested Types

    private class VirtualItemCollection : VirtualItemCollection<TControl, T>
    {
      #region Ctors

      public VirtualItemCollection(ItemCollectionBase<TControl, T> itemCollection)
      {
        ItemCollection = itemCollection;
      }

      #endregion

      #region Properties

      private ItemCollectionBase<TControl, T> ItemCollection { get; }

      #endregion

      #region  Methods

      protected override void ObservableSourceOnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
        base.ObservableSourceOnCollectionChanged(e);

        ItemCollection.OnSourceCollectionChanged(e);
      }

      protected override void OnGeneratedItemAttached(int index, T item)
      {
        ItemCollection.AttachGeneratedItem(index, item);
      }

      protected override void OnGeneratedItemDetached(int index, T item)
      {
        ItemCollection.DetachGeneratedItem(index, item);
      }

      #endregion
    }

    #endregion
  }

  internal interface IVirtualItemCollection
  {
    #region Properties

    int Count { get; }

    IVirtualItemsHost ItemHost { get; set; }

    #endregion

    #region  Methods

    void EnterGeneration();

    int GetIndexFromItem(FrameworkElement frameworkElement);

    void LeaveGeneration();

    UIElement Realize(int index);

    UIElement GetCurrent(int index);

    #endregion
  }
}