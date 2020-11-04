// <copyright file="ResourceSkinBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
  public abstract class ResourceSkinBase : DependencyObject, IResourceValue
  {
    #region Fields

    private string _actualKey;
    private SkinResourceManager _manager;
    internal event EventHandler ResourceChanged;

    #endregion

    #region Properties

    internal string ActualKey
    {
      get => _actualKey;
      set
      {
        if (string.Equals(_actualKey, value, StringComparison.OrdinalIgnoreCase))
          return;

        _actualKey = value;
      }
    }

    protected abstract object FrozenValue { get; }

    internal object FrozenValueInternal => FrozenValue;

    internal SkinResourceManager Manager
    {
      get => _manager;
      set
      {
        if (ReferenceEquals(_manager, value))
          return;

        _manager = value;
      }
    }

    protected abstract IEnumerable<DependencyProperty> Properties { get; }

    protected abstract object Value { get; }

    #endregion

    #region  Methods

    protected virtual void OnResourceChanged()
    {
      ResourceChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Interface Implementations

    #region IResourceValue

    string IResourceValue.Key
    {
      get => ActualKey;
      set => ActualKey = value;
    }

    #endregion

    #endregion
  }
}