// <copyright file="Converters.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;

namespace Zaaml.Core.Converters
{
  [AttributeUsage(AttributeTargets.Assembly)]
  internal class ConvertersAssemblyAttribute : Attribute
  {
  }

  internal interface IPrimitiveConverter
  {
    #region Properties

    Type FromType { get; }
    Type ToType { get; }

    #endregion

    #region  Methods

    object Convert(object value);

    object Convert(object value, IFormatProvider formatProvider);

    #endregion
  }

  internal static class PrimitiveConverterExtensions
  {
    #region  Methods

    public static T Convert<T>(this IPrimitiveConverter converter, object value)
    {
      if (converter.FromType != value.GetType() || converter.ToType != typeof(T))
        throw new InvalidOperationException("Converter types mismatch");

      return (T) converter.Convert(value);
    }

    public static IPrimitiveConverter<TFrom, TTo> ToGeneric<TFrom, TTo>(this IPrimitiveConverter converter)
    {
      return PrimitiveConverterFactory.GetWrappedConverter<TFrom, TTo>(converter);
    }

    #endregion
  }

  internal interface IPrimitiveConverter<in TFrom, out TTo> : IPrimitiveConverter
  {
    #region  Methods

    TTo Convert(TFrom value);
    TTo Convert(TFrom value, IFormatProvider formatProvider);

    #endregion
  }

  internal abstract class PrimitiveConverter : IPrimitiveConverter
  {
    #region Ctors

    protected PrimitiveConverter(Type fromType, Type toType)
    {
      FromType = fromType;
      ToType = toType;
    }

    #endregion

    #region Interface Implementations

    #region IPrimitiveConverter

    public abstract object Convert(object value);
    public abstract object Convert(object value, IFormatProvider formatProvider);

    public Type FromType { get; }
    public Type ToType { get; }

    #endregion

    #endregion
  }

  internal abstract class PrimitiveConverter<TFrom, TTo> : IPrimitiveConverter<TFrom, TTo>
  {
    #region Interface Implementations

    #region IPrimitiveConverter

    object IPrimitiveConverter.Convert(object value)
    {
      return Convert((TFrom) value);
    }

    object IPrimitiveConverter.Convert(object value, IFormatProvider formatProvider)
    {
      return Convert((TFrom) value, formatProvider);
    }

    public Type FromType => typeof(TFrom);

    public Type ToType => typeof(TTo);

    #endregion

    #region IPrimitiveConverter<TFrom,TTo>

    public abstract TTo Convert(TFrom value);
    public abstract TTo Convert(TFrom value, IFormatProvider formatProvider);

    #endregion

    #endregion
  }

  internal class GenericConverterWrapper<TFrom, TTo> : IPrimitiveConverter<TFrom, TTo>
  {
    #region Fields

    private readonly IPrimitiveConverter _converter;

    #endregion

    #region Ctors

    public GenericConverterWrapper(IPrimitiveConverter converter)
    {
      _converter = converter;

      if (converter.FromType != typeof(TFrom) || converter.ToType != typeof(TTo))
        throw new ArgumentException("converter");
    }

    #endregion

    #region Interface Implementations

    #region IPrimitiveConverter

    object IPrimitiveConverter.Convert(object value)
    {
      return _converter.Convert(value);
    }

    object IPrimitiveConverter.Convert(object value, IFormatProvider formatProvider)
    {
      return _converter.Convert(value, formatProvider);
    }

    public Type FromType => _converter.FromType;

    public Type ToType => _converter.ToType;

    #endregion

    #region IPrimitiveConverter<TFrom,TTo>

    public TTo Convert(TFrom value)
    {
      return (TTo) _converter.Convert(value);
    }

    public TTo Convert(TFrom value, IFormatProvider formatProvider)
    {
      return (TTo) _converter.Convert(value, formatProvider);
    }

    #endregion

    #endregion
  }

  internal class StringEnumConverter<TEnum> : PrimitiveConverter<string, TEnum>
  {
    #region Fields

    private readonly StringEnumConverter _converter;

    #endregion

    #region Ctors

    public StringEnumConverter()
    {
      _converter = new StringEnumConverter(typeof(Enum));
    }

    #endregion

    #region  Methods

    public override TEnum Convert(string value)
    {
      return (TEnum) _converter.Convert(value);
    }

    public override TEnum Convert(string value, IFormatProvider formatProvider)
    {
      return (TEnum) _converter.Convert(value, formatProvider);
    }

    #endregion
  }

  internal class StringEnumConverter : IPrimitiveConverter
  {
    #region Ctors

    public StringEnumConverter(Type enumType)
    {
      ToType = enumType;
    }

    #endregion

    #region Interface Implementations

    #region IPrimitiveConverter

    public object Convert(object value)
    {
      return Enum.Parse(ToType, (string) value, true);
    }

    public object Convert(object value, IFormatProvider formatProvider)
    {
      return Convert(value);
    }

    public Type FromType => typeof(string);

    public Type ToType { get; }

    #endregion

    #endregion
  }

  internal class EnumStringConverter : IPrimitiveConverter
  {
    #region Ctors

    public EnumStringConverter(Type enumType)
    {
      FromType = enumType;
    }

    #endregion

    #region Interface Implementations

    #region IPrimitiveConverter

    public object Convert(object value)
    {
      if (value.GetType().IsEnum == false)
        throw new ArrayTypeMismatchException("value");

      return value.ToString();
    }

    public object Convert(object value, IFormatProvider formatProvider)
    {
      if (value.GetType().IsEnum == false)
        throw new ArrayTypeMismatchException("value");

      return string.Format(formatProvider, "{0}", value);
    }

    public Type FromType { get; }

    public Type ToType => typeof(string);

    #endregion

    #endregion
  }

  internal class DelegatePrimitiveConverter<TFrom, TTo> : PrimitiveConverter<TFrom, TTo>
  {
    #region Fields

    private readonly Func<TFrom, TTo> _converter;
    private readonly Func<TFrom, IFormatProvider, TTo> _formattedConverter;

    #endregion

    #region Ctors

    //public DelegatePrimitiveConverter(Func<TFrom, TTo> converter):this(converter, null)
    //{
    //}

    public DelegatePrimitiveConverter(Func<TFrom, TTo> converter, Func<TFrom, IFormatProvider, TTo> formattedConverter)
    {
      _converter = converter;
      _formattedConverter = formattedConverter;
    }

    #endregion

    #region  Methods

    public override TTo Convert(TFrom value)
    {
      return _converter(value);
    }

    public override TTo Convert(TFrom value, IFormatProvider formatProvider)
    {
      return _formattedConverter == null ? _converter(value) : _formattedConverter(value, formatProvider);
    }

    #endregion
  }

  internal static class PrimitiveConverterFactory
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Tuple<RuntimeTypeHandle, RuntimeTypeHandle>, IPrimitiveConverter> KnownConverters =
      new Dictionary<Tuple<RuntimeTypeHandle, RuntimeTypeHandle>, IPrimitiveConverter>();

    private static readonly Dictionary<IPrimitiveConverter, IPrimitiveConverter> WrappedConverters =
      new Dictionary<IPrimitiveConverter, IPrimitiveConverter>();

    #endregion

    #region Ctors

    static PrimitiveConverterFactory()
    {
      RegisterTwoWaySimpleConverter<string, double>();
      RegisterTwoWaySimpleConverter<string, int>();
      RegisterTwoWaySimpleConverter<string, short>();
      RegisterTwoWaySimpleConverter<string, decimal>();
      RegisterTwoWaySimpleConverter<string, bool>();
    }

    #endregion

    #region  Methods

    private static IPrimitiveConverter<TInput, TOutput> CreateSimpleConverter<TInput, TOutput>()
    {
      return new DelegatePrimitiveConverter<TInput, TOutput>(value => (TOutput) Convert.ChangeType(value, typeof(TOutput), null), (value, format) => (TOutput) Convert.ChangeType(value, typeof(TOutput), format));
    }

    public static IPrimitiveConverter GetConverter(Type fromType, Type toType)
    {
      return KnownConverters.GetValueOrDefault(Tuple.Create(fromType.TypeHandle, toType.TypeHandle)) ??
             TryCreateEnumStringConverter(fromType, toType);
    }

    public static IPrimitiveConverter GetConverter(this object value, Type targetType)
    {
      return value != null ? GetConverter(value.GetType(), targetType) : null;
    }

    public static IPrimitiveConverter GetConverter<TTo>(this object value)
    {
      return value != null ? GetConverter(value.GetType(), typeof(TTo)) : null;
    }

    public static IPrimitiveConverter<TFrom, TTo> GetConverter<TFrom, TTo>()
    {
      var primitiveConverter = GetConverter(typeof(TFrom), typeof(TTo));

      return primitiveConverter.Return(p => p.As<IPrimitiveConverter<TFrom, TTo>>() ?? p.ToGeneric<TFrom, TTo>());
    }

    internal static IPrimitiveConverter<TFrom, TTo> GetWrappedConverter<TFrom, TTo>(IPrimitiveConverter converter)
    {
      return (IPrimitiveConverter<TFrom, TTo>) WrappedConverters.GetValueOrCreate(converter, () => new GenericConverterWrapper<TFrom, TTo>(converter));
    }

    public static IPrimitiveConverter RegisterConverter(IPrimitiveConverter converter)
    {
      KnownConverters.Add(Tuple.Create(converter.FromType.TypeHandle, converter.ToType.TypeHandle), converter);

      return converter;
    }

    private static void RegisterSimpleConverter<TInput, TOutput>()
    {
      RegisterConverter(CreateSimpleConverter<TInput, TOutput>());
    }

    private static void RegisterTwoWaySimpleConverter<TInput, TOutput>()
    {
      RegisterSimpleConverter<TInput, TOutput>();
      RegisterSimpleConverter<TOutput, TInput>();
    }

    private static IPrimitiveConverter TryCreateEnumStringConverter(Type fromType, Type toType)
    {
      if (fromType.IsEnum && toType == typeof(string))
        return RegisterConverter(new EnumStringConverter(fromType));

      if (fromType == typeof(string) && toType.IsEnum)
        return RegisterConverter(new StringEnumConverter(toType));

      return null;
    }

    #endregion
  }
}