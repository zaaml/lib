// <copyright file="ContextBarSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public abstract class ContextBarSelector : InheritanceContextObject, IPopupControllerSelector, ISharedItem
  {
    #region Ctors

    protected ContextBarSelector()
    {
      Owners = new SharedContextControlSelector(this);
    }

    #endregion

    #region Properties

    private SharedContextControlSelector Owners { get; }

    #endregion

    #region  Methods

    protected void RegisterContextBar(ContextBar menu)
    {
      Owners.RegisterSharedItem(menu);
    }

    public abstract ContextBar Select(FrameworkElement barOwner, DependencyObject eventSource);

    protected void UnregisterContextBar(ContextBar menu)
    {
      Owners.UnregisterSharedItem(menu);
    }

    #endregion

    #region Interface Implementations

    #region IPopupControllerSelector

    PopupControlController IPopupControllerSelector.SelectController(FrameworkElement frameworkElement, DependencyObject eventSource)
    {
      return Select(frameworkElement, eventSource)?.PopupController;
    }

    #endregion

    #region ISharedItem

    bool ISharedItem.IsShared { get; set; }

    SharedItemOwnerCollection ISharedItem.Owners => Owners;

    #endregion

    #endregion
  }
}