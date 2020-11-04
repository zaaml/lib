// <copyright file="PropertyChangedEventArgsXm.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  public class PropertyValueChangedEventArgs : EventArgs
  {
    #region Fields

    public readonly object NewValue;
    public readonly object OldValue;
    public readonly object Property;
    public readonly object Source;

    #endregion

    #region Ctors

    public PropertyValueChangedEventArgs(object source, object oldValue, object newValue, object property)
    {
      OldValue = oldValue;
      NewValue = newValue;
      Property = property;
      Source = source;
    }

    #endregion
  }

  public class PropertyValueChangedEventArgs<T> : EventArgs
  {
    #region Fields

    public readonly T NewValue;
    public readonly T OldValue;
    public readonly object Property;
    public readonly object Source;

    #endregion

    #region Ctors

    public PropertyValueChangedEventArgs(object source, T oldValue, T newValue, object property)
    {
      OldValue = oldValue;
      NewValue = newValue;
      Property = property;
      Source = source;
    }

    #endregion
  }
}