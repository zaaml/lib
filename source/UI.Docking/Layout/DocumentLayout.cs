// <copyright file="DocumentLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DocumentLayout : TabLayoutBase
  {
    #region Properties

    public override LayoutKind LayoutKind => LayoutKind.Document;

    #endregion
  }
}