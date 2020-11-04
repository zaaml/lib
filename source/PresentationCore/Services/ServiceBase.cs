// <copyright file="ServiceBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Services
{
  internal abstract class ServiceBase<T> : IDependencyObjectService<T> where T : DependencyObject
  {
    #region Properties

    public bool IsAttached => Target != null;

    public T Target { get; private set; }

    #endregion

    #region  Methods

    private IDependencyObjectService<T> Attach(T target)
    {
      Target = target;

      OnAttach();
      return this;
    }

    private IDependencyObjectService<T> Detach()
    {
      OnDetach();

      Target = null;
      return this;
    }

    protected virtual void OnAttach()
    {
    }

    protected virtual void OnDetach()
    {
    }

    #endregion

    #region Interface Implementations

    #region IDependencyObjectService

    IDependencyObjectService IDependencyObjectService.Attach(DependencyObject dependencyObject)
    {
      return Attach((T) dependencyObject);
    }

    IDependencyObjectService IDependencyObjectService.Detach(DependencyObject dependencyObject)
    {
      return Detach();
    }

    #endregion

    #region IDependencyObjectService<T>

    IDependencyObjectService<T> IDependencyObjectService<T>.Attach(T dependencyObject)
    {
      return Attach(dependencyObject);
    }

    IDependencyObjectService<T> IDependencyObjectService<T>.Detach(T dependencyObject)
    {
      return Detach();
    }

    bool IDependencyObjectService<T>.IsAttached => IsAttached;

    #endregion

    #region IDisposable

    public virtual void Dispose()
    {
    }

    #endregion

    #endregion
  }
}