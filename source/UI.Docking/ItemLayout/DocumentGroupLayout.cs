// <copyright file="DocumentGroupLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DocumentDockItemGroupLayout : DockItemGroupLayout
  {
    #region Ctors

    public DocumentDockItemGroupLayout(DocumentDockItemGroup groupItem)
      : base(groupItem)
    {
    }

    public DocumentDockItemGroupLayout()
    {
    }

    internal DocumentDockItemGroupLayout(DocumentDockItemGroupLayout groupLayout, DockItemLayoutCloneMode mode) : base(groupLayout, mode)
    {
    }

    #endregion

    #region Properties

    internal override DockItemGroupKind GroupKind => DockItemGroupKind.Document;

    #endregion

    #region  Methods

    internal override DockItemLayout CloneCore(DockItemLayoutCloneMode mode)
    {
      return new DocumentDockItemGroupLayout(this, mode);
    }

    #endregion
  }
}