// <copyright file="DocumentGroupTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DocumentDockItemGroupTemplateContract : DockItemTemplateContract
  {
    #region Properties

    [TemplateContractPart]
    public DocumentLayoutView LayoutView { get; [UsedImplicitly] private set; }

    #endregion
  }
}