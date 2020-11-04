// <copyright file="ContextControlNameScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using NameScope = Zaaml.PresentationCore.NameScope;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal abstract class ContextControlNameScope<T> : NameScope where T : FrameworkElement, IContextPopupControlInternal
  {
    #region Ctors

    protected ContextControlNameScope(T contextControl)
    {
      ContextControl = contextControl;
    }

    #endregion

    #region Properties

    public T ContextControl { get; }

    #endregion

    #region  Methods

    public override object FindName(string name)
    {
      if (ContextControl.Name == name)
        return this;

      var findName = base.FindName(name);

      if (findName != null)
        return findName;

      var parentFindName = this.GetVisualParent<FrameworkElement>()?.FindName(name);
      if (parentFindName != null)
        return parentFindName;

      var ownerFindName = ContextControl.Owner?.FindName(name);
      if (ownerFindName != null)
        return ownerFindName;

      foreach (var owner in ContextControl.Owners)
      {
        findName = owner.FindName(name);
        if (findName != null)
          return findName;
      }

      return null;
    }

    #endregion
  }
}
