// <copyright file="ContextBar.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public partial class ContextBar
  {
    #region  Methods

    partial void PlatformOnIsSharedChanged()
    {
      if (IsShared)
        NameScope.SetNameScope(this, new ContextBarNameScope(this));
      else
      {
        var nameScope = NameScope.GetNameScope(this) as ContextBarNameScope;
        if (ReferenceEquals(this, nameScope?.ContextControl))
          ClearValue(NameScope.NameScopeProperty);
      }
    }

    #endregion

    #region  Nested Types

    private sealed class ContextBarNameScope : ContextControlNameScope<ContextBar>
    {
      #region Ctors

      public ContextBarNameScope(ContextBar contextControl) : base(contextControl)
      {
      }

      #endregion
    }

    #endregion
  }
}
