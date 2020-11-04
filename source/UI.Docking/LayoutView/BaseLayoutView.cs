// <copyright file="BaseLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
  public abstract class BaseLayoutView : TemplateContractContentControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DockItem, BaseLayoutView>
      ("SelectedItem", v => v.OnSelectedItemChangedPrivate);

    #endregion

    #region Fields

    private bool _isArrangeValid;
    private BaseLayout _layoutInternal;

    #endregion

    #region Ctors

    protected BaseLayoutView()
    {
      Items = new DockItemCollection(OnItemAdded, OnItemRemoved);
    }

    #endregion

    #region Properties

    protected internal DockItemCollection Items { get; }

    internal BaseLayout LayoutInternal
    {
      get => _layoutInternal;
      set
      {
        if (ReferenceEquals(_layoutInternal, value))
          return;

        if (_layoutInternal != null)
        {
          OnLayoutDetaching();

          Items.Clear();
        }

        _layoutInternal = value;

        if (_layoutInternal != null)
        {
          Items.AddRange(_layoutInternal.Items);

          OnLayoutAttached();
        }
      }
    }

    protected internal IEnumerable<DockItem> OrderedItems => Items.OrderBy(w => LayoutInternal.GetDockItemIndex(w) ?? -1);

    protected internal IEnumerable<DockItem> ReverseOrderedItems => Items.OrderByDescending(w => LayoutInternal.GetDockItemIndex(w) ?? -1);

    public DockItem SelectedItem
    {
      get => (DockItem) GetValue(SelectedItemProperty);
      set => SetValue(SelectedItemProperty, value);
    }

    #endregion

    #region  Methods

    protected internal abstract void ArrangeItems();

    protected virtual void InvalidateItemsArrange()
    {
      if (_isArrangeValid == false)
        return;

      _isArrangeValid = false;

      InvalidateMeasure();
    }

    internal virtual bool IsItemVisible(DockItem item)
    {
      return true;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (_isArrangeValid)
        return base.MeasureOverride(availableSize);

      ArrangeItems();

      _isArrangeValid = true;

      return base.MeasureOverride(availableSize);
    }

    protected abstract void OnItemAdded(DockItem dockItem);

    protected abstract void OnItemRemoved(DockItem dockItem);

    protected virtual void OnLayoutAttached()
    {
      SetBinding(SelectedItemProperty, new Binding {Path = new PropertyPath(BaseLayout.SelectedItemProperty), Mode = BindingMode.TwoWay, Source = LayoutInternal});
    }

    protected virtual void OnLayoutDetaching()
    {
      ClearValue(SelectedItemProperty);
    }

    internal virtual void OnSelectedItemChangedInternal(DockItem oldItem, DockItem newItem)
    {
    }

    private void OnSelectedItemChangedPrivate(DockItem oldItem, DockItem newItem)
    {
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      InvalidateItemsArrange();
    }

    protected override void OnTemplateContractDetaching()
    {
      InvalidateItemsArrange();

      base.OnTemplateContractDetaching();
    }

    #endregion
  }

  public abstract class BaseLayoutView<T> : BaseLayoutView where T : BaseLayout
  {
    #region Properties

    protected T Layout => (T) LayoutInternal;

    #endregion
  }
}