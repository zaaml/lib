// <copyright file="XamlConvertException.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class XamlConvertException : Exception
  {
    #region Ctors

    internal XamlConvertException(object value, Type targetType, string message, Exception innerException) : base(message, innerException)
    {
      Value = value;
      TargetType = targetType;
    }

    #endregion

    #region Properties

    public Type TargetType { get; }

    public object Value { get; }

    #endregion
  }
}