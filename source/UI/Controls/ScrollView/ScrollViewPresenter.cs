// <copyright file="ScrollViewPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.ScrollView
{
  public class ScrollViewPresenter : ScrollViewPresenterBase<ScrollViewPanel>
  {
    #region Properties

    internal ScrollViewPanel ScrollViewPanel => TemplateRoot;

    #endregion
  }
}