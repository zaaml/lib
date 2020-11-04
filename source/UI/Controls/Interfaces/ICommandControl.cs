// <copyright file="ICommandControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;

namespace Zaaml.UI.Controls.Interfaces
{
  internal interface ICommandControl
  {
    #region Fields

    event EventHandler CommandChanged;
    event EventHandler CommandParameterChanged;
    event EventHandler CommandTargetChanged;

    #endregion

    #region Properties

    ICommand Command { get; set; }
    object CommandParameter { get; set; }
    DependencyObject CommandTarget { get; set; }

    #endregion
  }
}