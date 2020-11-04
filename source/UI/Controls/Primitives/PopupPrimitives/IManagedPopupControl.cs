// <copyright file="IManagedPopupControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal interface IManagedPopupControl
  {
    #region Properties

    DependencyProperty IsOpenProperty { get; }

    DependencyPropertyKey OwnerPropertyKey { get; }

    DependencyProperty PlacementProperty { get; }

    #endregion

    #region  Methods

    void OnClosed();

    void OnClosing(PopupCancelEventArgs cancelEventArgs);

    void OnIsOpenChanged();

    void OnOpened();

    void OnOpening(PopupCancelEventArgs cancelEventArgs);

    void OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner);

    void OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement);

    #endregion
  }
}