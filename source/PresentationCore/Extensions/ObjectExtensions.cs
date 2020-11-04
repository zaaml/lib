// <copyright file="ObjectExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class ObjectExtensions
  {
    #region  Methods

    public static object XamlConvert(this object value, Type targetType)
    {
      return XamlStaticConverter.ConvertValue(value, targetType);
    }

    #endregion
  }
}