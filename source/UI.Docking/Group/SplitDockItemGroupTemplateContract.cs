// <copyright file="SplitGroupTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class SplitDockItemGroupTemplateContract : DockItemTemplateContract
  {
    #region Properties

    [TemplateContractPart]
    public SplitLayoutView SplitLayoutView { get; [UsedImplicitly] private set; }

    #endregion
  }
}