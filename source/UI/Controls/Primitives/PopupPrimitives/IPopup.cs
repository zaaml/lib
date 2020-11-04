// <copyright file="IPopup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public interface IPopup
  {
    #region Fields

    event EventHandler IsOpenChanged;
    event EventHandler PlacementChanged;

    #endregion

    #region Properties

    bool IsOpen { get; set; }
    PopupPlacement Placement { get; set; }

    #endregion
  }
}