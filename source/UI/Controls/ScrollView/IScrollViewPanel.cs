// <copyright file="IScrollViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.ScrollView
{
  public interface IScrollViewPanel
  {
    #region Fields

    event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged;

    #endregion

    #region Properties

    bool CanHorizontallyScroll { get; set; }

    bool CanVerticallyScroll { get; set; }

    Size Extent { get; }

    Vector Offset { get; set; }

    Size Viewport { get; }

    #endregion

    #region  Methods

    void ExecuteScrollCommand(ScrollCommandKind command);

    #endregion
  }

  public interface IDelegateScrollViewPanel
  {
    #region Properties

    IScrollViewPanel ScrollViewPanel { get; }

    #endregion
  }
}