// <copyright file="BindingResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;



namespace Zaaml.PresentationCore.Data
{
  internal class BindingResolver : DependencyObject
  {
    #region Static Fields and Constants

    internal static readonly DependencyProperty InstanceProperty = DPM.RegisterAttached<BindingResolver, BindingResolver>
      ("Instance");

    internal static readonly DependencyProperty SourceProperty = DPM.Register<object, BindingResolver>
      ("Source", r => r.OnSourceResolved);

    #endregion

    #region Fields

    private readonly bool _autoDettach;
    private readonly Action<object> _bindingResolved;
    private readonly WeakReference _dataItemRef;
    private bool _isResolved;

    #endregion

    #region Ctors

    public BindingResolver(DependencyObject dataItem, Binding binding, Action<object> bindingResolved, bool autoDettach = true)
    {
      _dataItemRef = new WeakReference(dataItem);
      _bindingResolved = bindingResolved;
      _autoDettach = autoDettach;

      BindingResolverCollection.GetResolvers(dataItem).Add(this);
      BindingOperations.SetBinding(this, SourceProperty, binding);
    }

    #endregion

    #region  Methods

    private void OnSourceResolved(object oldValue, object newValue)
    {
      if (_isResolved)
        return;

      _bindingResolved(newValue);

      if (_autoDettach == false)
        return;

      _isResolved = true;

      ClearValue(SourceProperty);
      var dataItem = (DependencyObject) _dataItemRef.Target;

      if (dataItem == null) return;

      var resolvers = BindingResolverCollection.GetResolvers(dataItem);
      resolvers.Remove(this);
      if (resolvers.Count == 0)
        dataItem.ClearValue(BindingResolverCollection.ResolversProperty);
    }

    #endregion
  }
}