// <copyright file="TargetServiceProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Zaaml.PresentationCore
{
  internal sealed class TargetServiceProvider : IServiceProvider, IProvideValueTarget, IDisposable
  {
    #region Static Fields and Constants

    private static readonly Stack<TargetServiceProvider> ServiceProvidersPool = new Stack<TargetServiceProvider>(4);

    #endregion

    #region Fields

    private bool _isBusy;

    #endregion

    #region  Methods

    public static TargetServiceProvider GetServiceProvider(object targetObject, object targetProperty)
    {
      var targetServiceProvider = ServiceProvidersPool.Count > 0 ? ServiceProvidersPool.Pop() : new TargetServiceProvider();

      return targetServiceProvider.Setup(targetObject, targetProperty).Mount();
    }

    private TargetServiceProvider Mount()
    {
      if (_isBusy)
        throw new InvalidOperationException();

      _isBusy = true;
      return this;
    }

    private TargetServiceProvider Setup(object targetObject, object targetProperty)
    {
      TargetObject = targetObject;
      TargetProperty = targetProperty;

      return this;
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    void IDisposable.Dispose()
    {
      TargetObject = null;
      TargetProperty = null;

      if (_isBusy)
        ServiceProvidersPool.Push(this);
      else
        return;

      _isBusy = false;
    }

    #endregion

    #region IProvideValueTarget

    public object TargetObject { get; private set; }

    public object TargetProperty { get; private set; }

    #endregion

    #region IServiceProvider

    object IServiceProvider.GetService(Type serviceType)
    {
      return serviceType == typeof(IProvideValueTarget) ? this : null;
    }

    #endregion

    #endregion
  }
}