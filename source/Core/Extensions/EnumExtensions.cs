//  <copyright file="EnumExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Reflection;

namespace Zaaml.Core.Extensions
{
  internal static class EnumExtensions
  {
    #region  Methods

    public static bool IsValidValue(this Enum enumValue)
    {
      return Enum.IsDefined(enumValue.GetType(), enumValue);
    }

    public static void ValidateValue(this Enum enumValue, string exceptionMessage = null)
    {
      if (enumValue.IsValidValue() == false)
        throw new ArgumentOutOfRangeException(exceptionMessage);
    }

    public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum input)
    {
      var enumType = typeof (TEnum);

      if (!enumType.IsEnum)
        yield break;

      if (enumType.HasAttribute<FlagsAttribute>(false) == false)
        yield break;

      var enumValues = Enum.GetValues(enumType).OfType<TEnum>().ToList();
      var setBits = Convert.ToUInt64(input);

      if (setBits == 0)
      {
        foreach (var enumValue in enumValues.Where(enumValue => Convert.ToUInt64(enumValue) == 0))
        {
          yield return enumValue;

          break;
        }

        yield break;
      }

      foreach (var value in enumValues)
      {
        var valMask = Convert.ToUInt64(value);

        if ((setBits & valMask) == valMask)
          yield return value;
      }
    }

    #endregion
  }
}