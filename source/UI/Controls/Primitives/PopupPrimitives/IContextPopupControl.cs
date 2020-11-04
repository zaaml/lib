// <copyright file="IContextPopupControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public interface IContextPopupControl
  {
    #region Fields

    event EventHandler IsOpenChanged;

    #endregion

    #region Properties

    bool IsOpen { get; set; }

    #endregion
  }
}
