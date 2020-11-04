// <copyright file="DockController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DockController : DockControllerBase
  {
    #region Ctors

    public DockController(DockControlView controlView) : base(controlView)
    {
    }

    #endregion

    #region Properties

    protected override bool IsPreview => false;

    #endregion
  }
}