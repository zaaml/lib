// <copyright file="IApplicationMenu.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Ribbon
{
  public interface IApplicationMenu
  {
    #region Fields

    event EventHandler IsOpenChanged;

    #endregion

    #region Properties

    bool IsOpen { get; set; }

    #endregion
  }
}