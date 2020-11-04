// <copyright file="IInteractivityRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
  internal interface IInteractivityRoot : IServiceProvider, IInteractivityObject, IXamlRootOwner
  {
    #region Properties

    InteractivityService InteractivityService { get; }

    FrameworkElement InteractivityTarget { get; }

    #endregion
  }
}