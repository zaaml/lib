// <copyright file="PropertyUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Interfaces;

namespace Zaaml.Core.Utils
{
  internal static class PropertyUtils
  {
    #region  Methods

    internal static void ChangeNotifiableProperty<T>(this IRaiseOnPropertyChanged obj, ref T destination, T newValue, Action<T> beforeChange,
      Action<T> afterChange, string propertyName) where T : struct
    {
      ChangeProperty(ref destination, newValue, beforeChange, afterChange);
      obj.OnPropertyChanged(propertyName);
    }

    public static void ChangeNotifiableRefProperty<T>(ref T destination, T newValue, Action<string> onPropertyChange, Action beforeChange,
      Action afterChange,
      string propertyName) where T : class
    {
      ChangeRefProperty(ref destination, newValue, beforeChange, afterChange);
      onPropertyChange(propertyName);
    }

    public static void ChangeNotifiableRefProperty<T>(ref T destination, T newValue, Action<string> onPropertyChange, Action<T> beforeChange,
      Action<T> afterChange,
      string propertyName) where T : class
    {
      ChangeRefProperty(ref destination, newValue, beforeChange, afterChange);
      onPropertyChange(propertyName);
    }

    internal static void ChangeNotifiableRefProperty<T>(this IRaiseOnPropertyChanged obj, ref T destination, T newValue, Action<T> beforeChange,
      Action<T> afterChange, string propertyName) where T : class
    {
      ChangeRefProperty(ref destination, newValue, beforeChange, afterChange);
      obj.OnPropertyChanged(propertyName);
    }

    public static void ChangeProperty<T>(ref T destination, T newValue, Action<T> beforeChange = null, Action<T> afterChange = null) where T : struct
    {
      if (Equals(destination, newValue))
        return;

      beforeChange?.Invoke(destination);

      destination = newValue;

      afterChange?.Invoke(newValue);
    }

    public static void ChangeRefProperty<T>(ref T destination, T newValue, Action beforeChange, Action afterChange) where T : class
    {
      ChangeRefProperty(ref destination, newValue, _ => beforeChange(), _ => afterChange());
    }

    public static void ChangeRefProperty<T>(ref T destination, T newValue, Action<T> beforeChange = null, Action<T> afterChange = null) where T : class
    {
      if (ReferenceEquals(destination, newValue))
        return;

      beforeChange?.Invoke(destination);

      destination = newValue;

      afterChange?.Invoke(newValue);
    }

    public static void ChangeRefPropertyNotNullAction<T>(ref T destination, T newValue, Action<T> beforeChange = null, Action<T> afterChange = null)
      where T : class
    {
      if (ReferenceEquals(destination, newValue))
        return;

      if (beforeChange != null && destination != null)
        beforeChange(destination);

      destination = newValue;

      if (afterChange != null && newValue != null)
        afterChange(newValue);
    }

    #endregion
  }
}