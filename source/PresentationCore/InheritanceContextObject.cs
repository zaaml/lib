// <copyright file="InheritanceContextObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Data;

namespace Zaaml.PresentationCore
{
  public class InheritanceContextObject : Freezable
  {
    #region  Methods

		protected override Freezable CreateInstanceCore()
    {
      return null;
    }

    #endregion

    private IInheritanceContext _inheritanceContext;

    internal IInheritanceContext InheritanceContext
    {
      get => _inheritanceContext;
      set
      {
        if (ReferenceEquals(_inheritanceContext, value))
          return;

        if (_inheritanceContext != null)
          DetachContext(_inheritanceContext);

        _inheritanceContext = value;

        if (_inheritanceContext != null)
          AttachContext(_inheritanceContext);
      }
    }

    internal virtual void AttachContext(IInheritanceContext inheritanceContext)
    {
    }

    internal virtual void DetachContext(IInheritanceContext inheritanceContext)
    {
    }
  }
}