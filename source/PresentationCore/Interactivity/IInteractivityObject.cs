// <copyright file="IInteractivityObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Interactivity
{
  internal interface IInteractivityObject
  {
    #region Properties

    IInteractivityObject Parent { get; }

    #endregion

    #region  Methods

    void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName);

    #endregion
  }
}