// <copyright file="TypeExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
  internal static class TypeExtensions
  {
    #region  Methods

    public static object CreateDefaultValue(this Type type)
    {
      return RuntimeUtils.CreateDefaultValue(type);
    }

    public static bool IsSelfOrDerived<T>(this Type type)
    {
      var baseType = typeof(T);
      return type == baseType || type.IsSubclassOf(baseType);
    }

    #endregion
  }
}