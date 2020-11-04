// <copyright file="ValueChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  public class ValueChangedEventArgs<T> : EventArgs
  {
    #region Fields

    public readonly T NewValue;
    public readonly T OldValue;

    #endregion

    #region Ctors

    public ValueChangedEventArgs(T oldValue, T newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }

    #endregion
  }

  public class ValueChangedEventArgs : EventArgs
  {
    #region Methods

    public static ValueChangedEventArgs<T> Create<T>(T oldValue, T newValue)
    {
      return new ValueChangedEventArgs<T>(oldValue, newValue);
    }

    #endregion

    #region Fields

    public readonly object NewValue;
    public readonly object OldValue;

    #endregion

    #region Ctors

    public ValueChangedEventArgs(object oldValue, object newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }

    #endregion
  }


  public class ValueChangingEventArgs<T> : EventArgs
  {
    #region Fields

    public readonly T CurrentValue;
    public readonly T NewValue;

    #endregion

    #region Ctors

    public ValueChangingEventArgs(T currentValue, T newValue)
    {
      CurrentValue = currentValue;
      NewValue = newValue;
    }

    #endregion
  }

  public static class ValueChangingEventArgs
  {
    #region Methods

    public static ValueChangingEventArgs<T> Create<T>(T currentValue, T newValue)
    {
      return new ValueChangingEventArgs<T>(currentValue, newValue);
    }

    #endregion
  }
}