// <copyright file="TypeConversionUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.Core.Converters
{
  internal class ConvertibleTypeConverter<T> : TypeConverter where T : IConvertible
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType.GetInterface("IConvertible", false) != null;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType.GetInterface("IConvertible", false) != null;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      return ((IConvertible) value).ToType(typeof(T), CultureInfo.InvariantCulture);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
      Type destinationType)
    {
      return ((IConvertible) value).ToType(destinationType, CultureInfo.InvariantCulture);
    }

    #endregion
  }

  internal static class TypeConversionUtil
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Type, TypeConverter> ResolvedConverters = new Dictionary<Type, TypeConverter>();

    #endregion

    #region  Methods

    public static object ConvertFrom(TypeConverter converter, object value)
    {
      try
      {
        if (converter != null && value != null && converter.CanConvertFrom(value.GetType()))
          return converter.ConvertFrom(value);
      }
      catch (Exception ex)
      {
        LogService.LogError(ex);
      }

      return value;
    }

    private static TypeConverter CreateTypeConverter(Type type)
    {
      if (type.IsEnum)
        return new EnumTypeConverter(type);

      var converterAttributes = Attribute.GetCustomAttributes(type, typeof(TypeConverterAttribute), true);

      if (converterAttributes.Length != 0)
      {
	      foreach (var converterAttribute in converterAttributes.OfType<TypeConverterAttribute>())
	      {
		      try
		      {
			      var converterType = Type.GetType(converterAttribute.ConverterTypeName, false);

			      if (converterType != null)
				      return Activator.CreateInstance(converterType) as TypeConverter;
		      }
		      catch (Exception ex)
		      {
			      LogService.LogError(ex);
		      }
	      }
      }

      var conv = type.GetInterface("IConvertible", true);

      if (conv == null)
	      return null;

      try
      {
	      var d1 = typeof(ConvertibleTypeConverter<>);
	      Type[] typeArgs = { type };

	      var genericType = d1.MakeGenericType(typeArgs);

	      return (TypeConverter) Activator.CreateInstance(genericType);
      }
      catch (Exception e)
      {
	      LogService.LogError(e);
      }

      return null;
    }

    public static TypeConverter GetTypeConverter(Type type)
    {
      return ResolvedConverters.GetValueOrCreate(type, CreateTypeConverter);
    }

    #endregion
  }
}