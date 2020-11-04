// <copyright file="DockTabViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.TabView;

namespace Zaaml.UI.Controls.Docking
{
  public class DockTabViewItem : TabViewItem
  {
    #region Static Fields and Constants

    private static readonly RelayCommand<DockTabViewItem> StaticCloseCommand = new RelayCommand<DockTabViewItem>(OnStaticCloseCommandExecuted);

    #endregion

    #region Ctors

    static DockTabViewItem()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DockTabViewItem>();
    }

    public DockTabViewItem(DockItem dockItem)
    {
      this.OverrideStyleKey<DockTabViewItem>();

      DockItem = dockItem;

      SetBinding(HeaderProperty, new Binding {Path = new PropertyPath(DockItem.TitleProperty), Source = dockItem});
      SetBinding(IconProperty, new Binding {Path = new PropertyPath(DockItem.IconProperty), Source = dockItem});
      SetBinding(DisplayIndexProperty, new Binding {Path = new PropertyPath(BaseLayout.GetDockItemIndexProperty<TabLayout>()), Source = dockItem, Converter = DisplayIndexConverter.Instance, Mode = BindingMode.TwoWay});

      DataContext = dockItem;

      DragOutBehavior = new DragOutBehavior
      {
        DragOutCommand = new RelayCommand(OnDragOutCommandExecuted),
        ProcessHandledEvents = true,
        Target = this
      };

      CloseCommand = StaticCloseCommand;
      CloseCommandParameter = this;
    }

    #endregion

    #region Properties

    public DockItem DockItem { get; }

    [UsedImplicitly]
    private DragOutBehavior DragOutBehavior { get; }

    #endregion

    #region  Methods

    internal override void OnActivated()
    {
      base.OnActivated();

      DockItem.SelectAndFocus();
    }

    internal void OnAttachedInternal()
    {
      if (DockItem.IsSelected)
        IsSelected = true;
    }

    internal void OnDetachedInternal()
    {
    }

    private void OnDragOutCommandExecuted()
    {
      DockItem.DragOutInternal(DragOutBehavior.OriginMousePosition);
    }

    protected override void OnIsSelectedChanged()
    {
      base.OnIsSelectedChanged();

      if (IsSelected == false)
        return;

      DockItem.Select();
    }

    private static void OnStaticCloseCommandExecuted(DockTabViewItem dockTabViewItem)
    {
      dockTabViewItem.DockItem.DockState = DockItemState.Hidden;
    }

    #endregion

    #region  Nested Types

    private class DisplayIndexConverter : IValueConverter
    {
      #region Static Fields and Constants

      public static readonly DisplayIndexConverter Instance = new DisplayIndexConverter();

      #endregion

      #region Ctors

      private DisplayIndexConverter()
      {
      }

      #endregion

      #region Interface Implementations

      #region IValueConverter

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return (int?) value ?? int.MaxValue;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return value;
      }

      #endregion

      #endregion
    }

    #endregion
  }
}