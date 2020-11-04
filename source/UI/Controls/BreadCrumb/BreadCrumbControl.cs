// <copyright file="BreadCrumbControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Control = Zaaml.UI.Controls.Core.Control;

namespace Zaaml.UI.Controls.BreadCrumb
{
  [ContentProperty(nameof(Items))]
  public class BreadCrumbControl : Control, IBreadCrumbItemsOwner
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey SelectedItemPropertyKey = DPM.RegisterReadOnly<BreadCrumbItem, BreadCrumbControl>
      ("SelectedItem", b => b.OnSelectedItemChanged);

    public static readonly DependencyProperty ItemsIconVisibilityProperty = DPM.Register<ElementVisibility, BreadCrumbControl>
      ("ItemsIconVisibility", ElementVisibility.Visible, b => b.OnItemsIconVisibilityChanged);

    public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<BreadCrumbItemCollection, BreadCrumbControl>
      ("ItemsInt");

    public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

    public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, BreadCrumbControl>
      ("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

    private static readonly DependencyPropertyKey HasItemsPropertyKey = DPM.RegisterReadOnly<bool, BreadCrumbControl>
      ("HasItems", b => b.OnHasItemsChanged);

    public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
    private static readonly ITreeEnumeratorAdvisor<BreadCrumbItem> BreadCrumbEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<BreadCrumbItem>(b => b.Items.GetEnumerator());

    #endregion

    #region Fields

    private BreadCrumbItem _currentMenuBreadCrumbItem;
    private Panel _hostPanel;

    public event RoutedPropertyChangedEventHandler<BreadCrumbItem> SelectedItemChanged;

    #endregion

    #region Ctors

    static BreadCrumbControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<BreadCrumbControl>();
    }

    public BreadCrumbControl()
    {
      this.OverrideStyleKey<BreadCrumbControl>();
      Items = new BreadCrumbItemCollection(this);
    }

    #endregion

    #region Properties

    internal BreadCrumbItem CurrentMenuBreadCrumbItem
    {
      get => _currentMenuBreadCrumbItem;
      set
      {
        if (ReferenceEquals(_currentMenuBreadCrumbItem, value))
          return;

        try
        {
          CurrentMenuBreadCrumbItemChanging = true;

          if (_currentMenuBreadCrumbItem != null)
            _currentMenuBreadCrumbItem.IsMenuOpen = false;

          _currentMenuBreadCrumbItem = value;

          if (_currentMenuBreadCrumbItem != null)
            _currentMenuBreadCrumbItem.IsMenuOpen = true;
        }
        finally
        {
          CurrentMenuBreadCrumbItemChanging = false;
        }
      }
    }

    internal bool CurrentMenuBreadCrumbItemChanging { get; set; }


    public bool HasItems
    {
      get => (bool) GetValue(HasItemsProperty);
      private set => this.SetReadOnlyValue(HasItemsPropertyKey, value);
    }


    public IconBase Icon
    {
      get => (IconBase) GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    public ElementVisibility ItemsIconVisibility
    {
      get => (ElementVisibility) GetValue(ItemsIconVisibilityProperty);
      set => SetValue(ItemsIconVisibilityProperty, value);
    }


    public BreadCrumbItem SelectedItem
    {
      get => (BreadCrumbItem) GetValue(SelectedItemProperty);
      internal set => this.SetReadOnlyValue(SelectedItemPropertyKey, value);
    }

    #endregion

    #region  Methods

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _hostPanel?.Children.Clear();
      _hostPanel = (Panel) GetTemplateChild("hostPanel");

      UpdateControl();
    }

    private void OnHasItemsChanged(bool oldValue, bool newValue)
    {
      if (newValue && SelectedItem == null)
        SelectedItem = Items.First();
      else if (newValue == false)
        SelectedItem = null;
    }

    protected virtual void OnItemAdded(BreadCrumbItem item)
    {
    }

    private void OnItemAddedInt(BreadCrumbItem item)
    {
      UpdateHasItems();
      OnItemAdded(item);
    }

    protected virtual void OnItemRemoved(BreadCrumbItem item)
    {
    }

    private void OnItemRemovedInt(BreadCrumbItem item)
    {
      UpdateHasItems();
      OnItemRemoved(item);
    }

    private void OnItemsIconVisibilityChanged()
    {
      foreach (var breadCrumbItem in BreadCrumbEnumeratorAdvisor.GetEnumerator(Items).Enumerate())
        breadCrumbItem.UpdateIconVisibility();
    }

    private void OnSelectedItemChanged(BreadCrumbItem oldItem, BreadCrumbItem newItem)
    {
      if (oldItem?.IsSelected == true)
        oldItem.IsSelected = false;

      UpdateControl();
      OnSelectionChanged(oldItem, newItem);
    }

    protected virtual void OnSelectionChanged(BreadCrumbItem oldItem, BreadCrumbItem newItem)
    {
      SelectedItemChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<BreadCrumbItem>(oldItem, newItem));
    }

    private void UpdateControl()
    {
      if (_hostPanel == null)
        return;

      _hostPanel.Children.Clear();

      var current = SelectedItem;

      while (current != null)
      {
        _hostPanel.Children.Insert(0, current);
        current = current.ParentItem;
      }
    }

    private void UpdateHasItems()
    {
      HasItems = Items.Count > 0;
    }

    #endregion

    #region Interface Implementations

    #region IBreadCrumbItemsOwner

    public BreadCrumbItemCollection Items
    {
      get => (BreadCrumbItemCollection) GetValue(ItemsProperty);
      private set => this.SetReadOnlyValue(ItemsPropertyKey, value);
    }

    void IBreadCrumbItemsOwner.OnItemAdded(BreadCrumbItem item)
    {
      OnItemAddedInt(item);
    }

    void IBreadCrumbItemsOwner.OnItemRemoved(BreadCrumbItem item)
    {
      OnItemRemovedInt(item);
    }

    #endregion

    #endregion
  }

  internal interface IBreadCrumbItemsOwner
  {
    #region Properties

    BreadCrumbItemCollection Items { get; }

    #endregion

    #region  Methods

    void OnItemAdded(BreadCrumbItem item);

    void OnItemRemoved(BreadCrumbItem item);

    #endregion
  }
}