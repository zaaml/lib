// <copyright file="DockItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public class DockItemTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = false)]
    public DockItemContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = false)]
    public DockItemHeaderPresenter HeaderPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}