// <copyright file="IPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IPanel : IFrameworkElement
  {
    #region Properties

    IReadOnlyList<UIElement> Elements { get; }

    void AttachLayoutListener(IPanelLayoutListener layoutListener);

    void DetachLayoutListener(IPanelLayoutListener layoutListener);

    bool UseLayoutRounding { get; }

    #endregion
  }
}