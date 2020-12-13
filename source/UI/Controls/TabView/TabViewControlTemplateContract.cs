// <copyright file="TabViewControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
  public class TabViewControlTemplateContract : IndexedSelectorBaseTemplateContract<TabViewItemsPresenter>
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public TabViewContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}