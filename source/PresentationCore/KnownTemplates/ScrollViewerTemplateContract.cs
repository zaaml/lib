//  <copyright file="ScrollViewerTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.PresentationCore.KnownTemplates
{
  public sealed class ScrollViewerTemplateContract : TemplateContract
  {
    #region Ctors

    public ScrollViewerTemplateContract(ScrollViewer frameworkElement)
      : base(frameworkElement)
    {
    }

    #endregion

    #region Properties

    [TemplateContractPart]
    public ScrollBar VerticalScrollBar { get; [UsedImplicitly] private set; }

    [TemplateContractPart]
    public ScrollBar HorizontalScrollBar { get; [UsedImplicitly] private set; }

    [TemplateContractPart]
    public ScrollContentPresenter ScrollContentPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}