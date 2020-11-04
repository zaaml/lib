// <copyright file="ScrollViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.ScrollView
{
  public sealed class ScrollViewPanel : ScrollViewPanelBase
  {
    #region Properties

    protected override ScrollViewControlBase ScrollView => ViewPresenter?.ScrollView as ScrollViewControl;

    private ScrollViewPresenter ViewPresenter => this.GetVisualParent() as ScrollViewPresenter;

    #endregion
  }
}