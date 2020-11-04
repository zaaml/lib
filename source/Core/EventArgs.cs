// <copyright file="EventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  public class EventArgs<T> : EventArgs
  {
    #region Ctors

    public EventArgs(T value)
    {
      Value = value;
    }

    #endregion

    #region Properties

    public T Value { get; }

    #endregion
  }
}