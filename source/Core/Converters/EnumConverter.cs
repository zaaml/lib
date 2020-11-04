// <copyright file="EnumConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq.Expressions;

namespace Zaaml.Core.Converters
{
  internal static class EnumConverter<TEnum> where TEnum : struct, Enum, IConvertible
  {
    #region Static Fields and Constants

    private static readonly Func<long, TEnum> DirectConverter = GenerateConverter<long, TEnum>();
    private static readonly Func<TEnum, long> ReverseConverter = GenerateConverter<TEnum, long>();

    #endregion

    #region  Methods

    public static TEnum Convert(long value)
    {
      return DirectConverter(value);
    }

    public static long Convert(TEnum value)
    {
      return ReverseConverter(value);
    }

    private static Func<TInput, TOutput> GenerateConverter<TInput, TOutput>()
    {
      var parameter = Expression.Parameter(typeof(TInput));
      var dynamicMethod = Expression.Lambda<Func<TInput, TOutput>>(Expression.Convert(parameter, typeof(TOutput)), parameter);

      return dynamicMethod.Compile();
    }

    #endregion
  }
}