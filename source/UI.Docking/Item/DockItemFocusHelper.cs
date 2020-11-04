// <copyright file="DockItemFocusHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Docking
{
  internal static class DockItemFocusHelper
  {
    #region Static Fields and Constants

    private static WeakReference _focusedDockItemWeakReference;

    #endregion

    #region Ctors

    static DockItemFocusHelper()
    {
      FocusObserver.KeyboardFocusedElementChanged += FocusObserverOnKeyboardFocusedElementChanged;
    }

    #endregion

    #region Properties

    public static DockItem FocusedItem
    {
      get => (DockItem) _focusedDockItemWeakReference?.Target;
      set
      {
        var focusedItem = FocusedItem;

        if (ReferenceEquals(focusedItem, value))
          return;

        _focusedDockItemWeakReference = null;

        focusedItem?.OnLostKeyboardFocusInternal();

        _focusedDockItemWeakReference = value != null ? new WeakReference(value) : null;

        value?.OnGotKeyboardFocusInternal();
      }
    }

    #endregion

    #region  Methods

    public static void FocusItem(DockItem dockItem)
    {
      FocusHelper.QueryKeyboardFocus(dockItem);
    }

    private static void FocusObserverOnKeyboardFocusedElementChanged(object sender, EventArgs e)
    {
      FocusedItem = GetKeyboardFocusItem();
    }

    private static DockItem GetKeyboardFocusItem()
    {
      var focusedElement = FocusObserver.KeyboardFocusedElement as DependencyObject;

      return focusedElement.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance).OfType<DockItem>().FirstOrDefault();
    }

    public static bool IsKeyboardFocusWithin(DockItem dockItem)
    {
      var isKeyboardFocusWithin = FocusHelper.IsKeyboardFocusWithin(dockItem) || dockItem.FloatingWindow != null && FocusHelper.IsKeyboardFocusWithin(dockItem.FloatingWindow);

      if (isKeyboardFocusWithin)
        return true;

      if (ReferenceEquals(FocusedItem, dockItem))
        return true;

      var focusedElement = FocusHelper.GetKeyboardFocusedElement() as DependencyObject;

      return focusedElement != null && focusedElement.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance).Any(a => ReferenceEquals(a, dockItem));
    }

    #endregion
  }
}