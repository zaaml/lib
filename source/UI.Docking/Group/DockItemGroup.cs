// <copyright file="DockItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  public abstract class DockItemGroup : DockItem
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DockItem, DockItemGroup>
      ("SelectedItem", g => g.OnSelectedItemChangedPrivate);

    #endregion

    #region Fields

    private DockItem _actualItem;
    private int _suspendUpdateActualItemCount;

    public event EventHandler SelectedItemChanged;

    #endregion

    #region Ctors

    protected DockItemGroup(DockItemState dockState, BaseLayout layout) : base(dockState)
    {
      Items = new DockItemCollection(OnItemAdded, OnItemRemoved, true);
      ActualItems = new Dictionary<DockItem, DockItem>();

      Layout = layout;
      Layout.SetBinding(BaseLayout.SelectedItemProperty, new Binding {Path = new PropertyPath(SelectedItemProperty), Mode = BindingMode.TwoWay, Source = this});
    }

    #endregion

    #region Properties

    protected internal override DockItem ActualItem => _actualItem;

    protected Dictionary<DockItem, DockItem> ActualItems { get; }

    internal virtual bool AllowSingleItem => false;

    internal override DockControl DockControl
    {
      get => base.DockControl;
      set
      {
        base.DockControl = value;

        Layout.DockControl = value;
      }
    }

    public abstract DockItemGroupKind GroupKind { get; }

    internal override bool IsActualSelected => Layout.SelectedItem?.IsSelected == true;

    public DockItemCollection Items { get; }

    internal BaseLayout Layout { get; }

    public DockItem SelectedItem
    {
      get => (DockItem) GetValue(SelectedItemProperty);
      set => SetValue(SelectedItemProperty, value);
    }

    #endregion

    #region  Methods

    protected IEnumerable<DockItem> ActualDescendants()
    {
      return Descendants().Select(EvaluateActualItem).SkipNull();
    }

    protected void AddActualItem(DockItem item)
    {
      RemoveActualItem(item);

      item.ActualItemChanged += ItemOnActualItemChanged;

      ActualItems.Add(item, item.ActualItem);

      UpdateActualItem();
    }

    internal void AddItems(IEnumerable<DockItem> items)
    {
      SuspendActualItemUpdate();

      foreach (var item in items)
        Items.Add(item);

      ReleaseActualItemUpdate();

      UpdateActualItem();
    }

    internal override void AttachController(DockControllerBase controller)
    {
      base.AttachController(controller);

      Layout.AttachController(controller);
    }

    internal void ClearItems()
    {
      SuspendActualItemUpdate();

      Items.Clear();

      ReleaseActualItemUpdate();

      UpdateActualItem();
    }

    internal IEnumerable<DockItem> Descendants(DockItemState state)
    {
      return Items.Where(w => w.DockState == state);
    }

    internal IEnumerable<DockItem> Descendants()
    {
      return Descendants(DockState);
    }

    internal override void DetachController(DockControllerBase controller)
    {
      Layout.DetachController(controller);

      base.DetachController(controller);
    }

    internal void EnsureItemsIndices()
    {
      foreach (var dockItem in Items)
        Layout.SetDockItemIndex(dockItem, Layout.GetDockItemIndex(dockItem) ?? Layout.IndexProvider.NewIndex);
    }

    private static DockItem EvaluateActualItem(DockItem dockItem)
    {
      while (true)
      {
        var dockItemGroup = dockItem as DockItemGroup;

        if (dockItemGroup == null)
          return dockItem;

        DockItem first = null;

        foreach (var descendant in dockItemGroup.Descendants())
        {
          if (first != null)
            return dockItemGroup;

          first = descendant;

          if (dockItemGroup.AllowSingleItem)
            return dockItemGroup;
        }

        dockItem = first;
      }
    }

    internal virtual BaseLayout GetItemTargetLayout(DockItem item, bool arrange) => ReferenceEquals(this, ActualItem) ? Layout : GetTargetLayout(item, arrange);

    internal override BaseLayout GetTargetLayout(DockItem item, bool arrange)
    {
      if (ReferenceEquals(this, item))
      {
        if (ReferenceEquals(ActualItem, this))
          return base.GetTargetLayout(item, arrange);

        return null;
      }

      return base.GetTargetLayout(item, arrange);
    }

    private void InvalidateDescendantArrange(DockItem item)
    {
      if (item.ActualItem != null && ReferenceEquals(item, ActualItem) == false)
        item.ActualItem.InvalidateItemArrange();

      item.InvalidateItemArrange();
    }

    private void ItemOnActualItemChanged(object sender, EventArgs eventArgs)
    {
      var item = (DockItem) sender;
      var oldActualItem = ActualItems[item];
      var newActualItem = item.ActualItem;

      ActualItems[item] = newActualItem;

      oldActualItem?.InvalidateItemArrange();
      newActualItem?.InvalidateItemArrange();

      UpdateActualItem();

      InvalidateItemArrange();
    }

    protected override void OnDockStateChanged(DockItemState oldState)
    {
      try
      {
        SuspendActualItemUpdate();

        foreach (var window in Descendants().ToList())
          Items.Remove(window);

        foreach (var item in Descendants(oldState).ToList())
        {
          item.DockState = DockState;

          Items.Remove(item);
          Items.Add(item);
        }
      }
      finally
      {
        ReleaseActualItemUpdate();

        UpdateActualItem();

        foreach (var item in Items)
          item.InvalidateItemArrange();

        base.OnDockStateChanged(oldState);
      }
    }

    protected virtual void OnItemAdded(DockItem item)
    {
      var parentGroup = item.GetParentGroup(DockState);

      parentGroup?.Items.Remove(item);

      item.AttachGroup(DockState, this);

      item.DockStateChanged += OnItemDockStateChanged;
      item.IsSelectedChanged += OnItemIsSelectedChanged;

      Layout.UpdateDockItemIndex(item);

      if (item.DockState == DockState)
        AddActualItem(item);

      InvalidateItemArrange();
      UpdateVisualStatePrivate();
    }

    private void OnItemDockStateChanged(object sender, DockItemStateChangedEventArgs args)
    {
      var item = (DockItem) sender;

      if (item.DockState != DockState)
        RemoveActualItem(item);
      else
        AddActualItem(item);

      InvalidateItemArrange();
      InvalidateDescendantArrange(item);
      UpdateVisualStatePrivate();
    }

    private void OnItemIsSelectedChanged(object sender, EventArgs e)
    {
      UpdateVisualStatePrivate();
    }

    protected virtual void OnItemRemoved(DockItem item)
    {
      item.DetachGroup(this);

      item.DockStateChanged -= OnItemDockStateChanged;
      item.IsSelectedChanged -= OnItemIsSelectedChanged;

      if (item.DockState == DockState)
        RemoveActualItem(item);

      InvalidateItemArrange();
      InvalidateDescendantArrange(item);
      UpdateVisualStatePrivate();
    }

    protected virtual void OnSelectedItemChanged(DockItem oldItem, DockItem newItem)
    {
    }

    internal virtual void OnSelectedItemChangedInternal(DockItem oldItem, DockItem newItem)
    {
      OnSelectedItemChanged(oldItem, newItem);

      SelectedItemChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnSelectedItemChangedPrivate(DockItem oldItem, DockItem newItem)
    {
      OnSelectedItemChangedInternal(oldItem, newItem);
    }

    internal void ReleaseActualItemUpdate()
    {
      if (_suspendUpdateActualItemCount == 0)
        return;

      _suspendUpdateActualItemCount--;

      if (_suspendUpdateActualItemCount == 0)
        UpdateActualItem();
    }

    protected void RemoveActualItem(DockItem item)
    {
      item.ActualItemChanged -= ItemOnActualItemChanged;

      ActualItems.Remove(item);

      UpdateActualItem();
    }

    internal void ReplaceItem(DockItem oldItem, DockItem newItem)
    {
      var actualItem = EvaluateActualItem(oldItem);

      // TODO ActualLayout could be wrong
      var index = actualItem?.GetTargetLayout(false)?.GetDockItemIndex(actualItem);

      SuspendActualItemUpdate();

      Items.Replace(oldItem, newItem);

      ReleaseActualItemUpdate();

      UpdateActualItem();

      // TODO ActualLayout could be wrong
      newItem.GetTargetLayout(false)?.SetDockItemIndex(newItem, index);
    }

    private void SetActualItem(DockItem value)
    {
      if (ReferenceEquals(_actualItem, value))
        return;

      if (_actualItem != null)
      {
        _actualItem.InvalidateItemArrange();

        if (ReferenceEquals(this, _actualItem) == false)
          DockItemLayoutMentorService.FromItem(_actualItem).RemoveMentor(this);
      }

      _actualItem = value;

      if (_actualItem != null)
      {
        _actualItem.InvalidateItemArrange();

        if (ReferenceEquals(this, _actualItem) == false)
          DockItemLayoutMentorService.FromItem(_actualItem).AddMentor(this);
      }

      OnActualItemChanged();
      InvalidateItemArrange();
    }

    internal void SuspendActualItemUpdate()
    {
      _suspendUpdateActualItemCount++;
    }

    private void UpdateActualItem()
    {
      if (_suspendUpdateActualItemCount > 0)
        return;

      DockItem firstActualItem = null;
      var count = 0;

      foreach (var kv in ActualItems.Where(kv => kv.Value != null))
      {
        count++;
        firstActualItem = firstActualItem ?? kv.Value;
      }

      SetActualItem(count > 1 || count == 1 && AllowSingleItem ? this : firstActualItem);
    }

    private void UpdateVisualStatePrivate()
    {
      UpdateVisualState(true);
    }

    #endregion

    #region  Nested Types

    protected class DockItemGroupPropertyMetadata : PropertyMetadata
    {
      #region Ctors

      public DockItemGroupPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback = null)
        : base(defaultValue, propertyChangedCallback)
      {
      }

      #endregion
    }

    #endregion
  }

  [ContentProperty(nameof(Items))]
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
  public abstract class DockItemGroup<T> : DockItemGroup where T : BaseLayout, new()
  {
    #region Ctors

    protected DockItemGroup(DockItemState dockState) : base(dockState, new T())
    {
    }

    #endregion

    #region Properties

    protected abstract BaseLayoutView<T> LayoutView { get; }

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      Layout.View = LayoutView;
    }

    protected override void OnTemplateContractDetaching()
    {
      Layout.View = null;

      base.OnTemplateContractDetaching();
    }

    #endregion
  }
}