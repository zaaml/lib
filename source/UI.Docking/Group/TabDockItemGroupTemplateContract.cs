// <copyright file="TabGroupTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class TabDockItemGroupTemplateContract : DockItemTemplateContract
  {
    #region Properties

    [TemplateContractPart]
    public TabLayoutView LayoutView { get; [UsedImplicitly] private set; }

    #endregion
  }
}