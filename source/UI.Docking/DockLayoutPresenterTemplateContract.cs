// <copyright file="DockLayoutPresenterTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public class DockLayoutPresenterTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart]
    public AutoHideLayoutView AutoHideLayoutView { get; [UsedImplicitly] private set; }

    [TemplateContractPart]
    public DockLayoutView DockLayoutView { get; [UsedImplicitly] private set; }

    [TemplateContractPart]
    public DocumentLayoutView DocumentLayoutView { get; [UsedImplicitly] private set; }

    [TemplateContractPart]
    public FloatLayoutView FloatLayoutView { get; [UsedImplicitly] private set; }

    #endregion
  }
}