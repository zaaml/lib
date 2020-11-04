// <copyright file="PropertyChangingEventArgsXm.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  public class PropertyChangingEventArgsXm : EventArgs
  {
    #region Fields

    public readonly object NewValue;
    public readonly object OldValue;
    public readonly object Property;
    public readonly object Source;

    #endregion

    #region Ctors

    public PropertyChangingEventArgsXm(object source, object oldValue, object newValue, object property)
    {
      OldValue = oldValue;
      NewValue = newValue;
      Property = property;
      Source = source;
    }

    #endregion

    #region Properties

    public bool Cancel { get; set; }

    #endregion
  }

  public class PropertyChangingEventArgsXm<T> : EventArgs
  {
    #region Fields

    public readonly T NewValue;
    public readonly T OldValue;
    public readonly object Property;
    public readonly object Source;

    #endregion

    #region Ctors

    public PropertyChangingEventArgsXm(object source, T oldValue, T newValue, object property)
    {
      OldValue = oldValue;
      NewValue = newValue;
      Property = property;
      Source = source;
    }

    #endregion

    #region Properties

    public bool Cancel { get; set; }

    #endregion
  }
}