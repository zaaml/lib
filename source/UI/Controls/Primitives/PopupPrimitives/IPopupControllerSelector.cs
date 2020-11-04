// <copyright file="IPopupControllerSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal interface IPopupControllerSelector
  {
    #region  Methods

    PopupControlController SelectController(FrameworkElement frameworkElement, DependencyObject eventSource);

    #endregion
  }
}