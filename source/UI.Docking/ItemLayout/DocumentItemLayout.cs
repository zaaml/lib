// <copyright file="DocumentItemLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DocumentDockItemLayout : DockItemLayout
  {
    #region Ctors

    public DocumentDockItemLayout(DocumentDockItem item) : base(item)
    {
    }

    public DocumentDockItemLayout()
    {
    }

    internal DocumentDockItemLayout(DockItemLayout source, DockItemLayoutCloneMode mode) : base(source, mode)
    {
    }

    #endregion

    #region  Methods

    internal override DockItemLayout CloneCore(DockItemLayoutCloneMode mode)
    {
      return new DocumentDockItemLayout(this, mode);
    }

    #endregion
  }
}