// <copyright file="IOwnedPopup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public interface IOwnedPopup : IPopup
  {
    #region Fields

    event EventHandler OwnerChanged;

    #endregion

    #region Properties

    FrameworkElement Owner { get; set; }

    #endregion
  }
}