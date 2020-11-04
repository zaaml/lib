// <copyright file="IToggleButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Interfaces
{
  internal interface IToggleButton : IButton
  {
    #region Fields

    event RoutedEventHandler Checked;
    event RoutedEventHandler Unchecked;
    event RoutedEventHandler Indeterminate;

    #endregion

    #region Properties

    bool? IsChecked { get; set; }

    DependencyProperty IsCheckedPropertyInt { get; }
    bool IsThreeState { get; set; }

    #endregion
  }
}