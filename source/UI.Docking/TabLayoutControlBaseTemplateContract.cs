// <copyright file="TabLayoutControlBaseTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public class TabLayoutViewBaseTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = false)]
    public Panel Host { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = false)]
    public DockTabViewControl TabViewControl { get; [UsedImplicitly] private set; }

    #endregion
  }
}