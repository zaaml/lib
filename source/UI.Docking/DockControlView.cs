// <copyright file="DockControlView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DockControlView : DockControlViewBase
  {
    #region Ctors

    public DockControlView()
    {
      Controller = new DockController(this);
    }

    #endregion

    #region Properties

    internal DockController Controller { get; }

    internal override DockControllerBase ControllerCore => Controller;

    #endregion
  }
}