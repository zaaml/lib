// <copyright file="TabLayoutViewBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.TabView;

namespace Zaaml.UI.Controls.Docking
{
  public abstract class TabLayoutViewBase<TLayout> : BaseLayoutView<TLayout> where TLayout : TabLayoutBase
  {
    #region Properties

    private Panel Host => TemplateContract.Host;

    private DockTabViewControl TabViewControl => TemplateContract.TabViewControl;

    private TabLayoutViewBaseTemplateContract TemplateContract => (TabLayoutViewBaseTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    protected internal override void ArrangeItems()
    {
    }

    private void AttachItem(DockItem item)
    {
      if (TabViewControl != null)
      {
        item.ActualSelectionScope.Suspend();

        var tabViewItem = item.TabViewItem;

        tabViewItem.Content = item;
        TabViewControl?.ItemCollection.Add(tabViewItem);

        item.ActualSelectionScope.Resume();

        tabViewItem.OnAttachedInternal();
      }
      else
        Host?.Children.Add(item);
    }

    protected override TemplateContract CreateTemplateContract()
    {
      return new TabLayoutViewBaseTemplateContract();
    }

    private void DetachItem(DockItem item)
    {
      if (TabViewControl != null)
      {
        item.ActualSelectionScope.Suspend();

        var tabViewItem = item.TabViewItem;

        item.TabViewItem.Content = null;
        TabViewControl?.ItemCollection.Remove(tabViewItem);

        item.ActualSelectionScope.Resume();

        tabViewItem.OnDetachedInternal();
      }
      else
        Host?.Children.Remove(item);
    }

    internal override bool IsItemVisible(DockItem item)
    {
      return TabViewControl != null ? item.TabViewItem.IsSelected : Host.Children.Contains(item);
    }

    protected override void OnItemAdded(DockItem item)
    {
      AttachItem(item);
    }

    protected override void OnItemRemoved(DockItem item)
    {
      DetachItem(item);
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      foreach (var item in Items)
        AttachItem(item);

      TabViewControl?.SetBinding(TabView.TabViewControl.SelectedItemProperty, new Binding
      {
        Path = new PropertyPath(SelectedItemProperty),
        Mode = BindingMode.TwoWay,
        Source = this,
        Converter = TabViewItemConverter.Instance
      });
    }

    protected override void OnTemplateContractDetaching()
    {
      TabViewControl?.ClearValue(TabView.TabViewControl.SelectedItemProperty);

      foreach (var item in Items)
        DetachItem(item);

      base.OnTemplateContractDetaching();
    }

    #endregion

    #region  Nested Types

    private sealed class TabViewItemConverter : IValueConverter
    {
      #region Static Fields and Constants

      public static readonly TabViewItemConverter Instance = new TabViewItemConverter();

      #endregion

      #region Ctors

      private TabViewItemConverter()
      {
      }

      #endregion

      #region  Methods

      private static object Convert(object value, Type targetType)
      {
        if (value == null)
          return null;

        if (typeof(TabViewItem).IsAssignableFrom(targetType) && value is DockItem)
          return ((DockItem) value).TabViewItem;

        if (typeof(DockItem).IsAssignableFrom(targetType) && value is DockTabViewItem)
          return ((DockTabViewItem) value).DockItem;

        throw new InvalidOperationException();
      }

      #endregion

      #region Interface Implementations

      #region IValueConverter

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return Convert(value, targetType);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return Convert(value, targetType);
      }

      #endregion

      #endregion
    }

    #endregion
  }
}