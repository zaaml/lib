// <copyright file="IDependencyObjectService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Services
{
  internal interface IDependencyObjectService<in T> : IDependencyObjectService where T : DependencyObject
  {
    #region Properties

    bool IsAttached { get; }

    #endregion

    #region  Methods

    IDependencyObjectService<T> Attach(T dependencyObject);
    IDependencyObjectService<T> Detach(T dependencyObject);

    #endregion
  }

  internal interface IDependencyObjectService : IDisposable
  {
    #region  Methods

    IDependencyObjectService Attach(DependencyObject dependencyObject);
    IDependencyObjectService Detach(DependencyObject dependencyObject);

    #endregion
  }
}