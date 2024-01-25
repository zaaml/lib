// <copyright file="XamlStaticConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Converters;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Converters
{
	internal static class XamlStaticConverter
	{
		static XamlStaticConverter()
		{
			RuntimeHelpers.RunClassConstructor(typeof(ValueConverters).TypeHandle);
		}

		private static bool CanCacheValueOfType(Type type)
		{
			return type.IsValueType;
		}

		public static object ConvertCache(ref ConvertCacheStore cacheStore, Type targetType)
		{
			return TryConvertCache(ref cacheStore, targetType).Result;
		}

		public static T ConvertValue<T>(object value)
		{
			return (T)ConvertValue(value, typeof(T));
		}

		public static object ConvertValue(object value, Type targetType)
		{
			var result = ConvertValueImpl(value, targetType, out var exception);

			if (exception != null)
				throw exception;

			return result;
		}

		private static object ConvertValueImpl(object value, Type targetType, out XamlConvertException exception)
		{
			try
			{
				exception = null;

				// Target type is undefined or type of object
				if (targetType == null || targetType == typeof(object))
					return value;

				// Unset or null value
				if (value == null || value.IsUnset())
					return targetType.CreateDefaultValue();

				var valueType = value.GetType();

				// No conversion is needed
				if (targetType.IsAssignableFrom(valueType))
					return value;

				// Primitive converter
				var primitiveConverter = value.GetConverter(targetType);

				if (primitiveConverter != null)
					return primitiveConverter.Convert(value, CultureInfo.InvariantCulture);

				var typeDescriptorContext = DummyTypeDescriptorContext.TypeDescriptorInstance;

				// Target Type converter
				var converter = TypeConversionUtil.GetTypeConverter(targetType);

				if (converter?.CanConvertFrom(typeDescriptorContext, valueType) == true)
					return converter.ConvertFrom(typeDescriptorContext, CultureInfo.InvariantCulture, value);

				// Source Type converter
				converter = TypeConversionUtil.GetTypeConverter(valueType);

				if (converter?.CanConvertTo(typeDescriptorContext, targetType) == true)
					return converter.ConvertTo(typeDescriptorContext, CultureInfo.InvariantCulture, value, targetType);

				// Xaml converter
				converter = XamlTypeConverter.GetConverter(targetType);

				if (converter?.CanConvertFrom(typeDescriptorContext, valueType) == true)
					return converter.ConvertFrom(typeDescriptorContext, CultureInfo.InvariantCulture, value);

				exception = CreateException(value, targetType, $"Can not convert value '{value}' to type {targetType}");
			}
			catch (Exception e)
			{
				exception = CreateException(value, targetType, e);
			}

			return null;
		}

		private static XamlConvertException CreateException(object value, Type targetType, Exception innerException)
		{
			return new XamlConvertException(value, targetType, $"Error occurred while converting value '{value}' to type {targetType}", innerException);
		}

		private static XamlConvertException CreateException(object value, Type targetType, string message)
		{
			return new XamlConvertException(value, targetType, message, null);
		}

		public static XamlConvertResult TryConvertCache(ref ConvertCacheStore cacheStore, Type targetType)
		{
			if (targetType == null)
				return new XamlConvertResult(null, CreateException(cacheStore.Value, null, "Target type is not specified"));

			if (cacheStore.CachedType == targetType)
				return new XamlConvertResult(cacheStore.CachedValue, null);

			var actualValue = cacheStore.Value.IsUnset() ? null : cacheStore.Value;

			var result = TryConvertValue(actualValue, targetType);

			cacheStore.CachedValue = result.IsValid ? result.Result : targetType.CreateDefaultValue();

			if (CanCacheValueOfType(targetType))
				cacheStore.CachedType = targetType;

			return result;
		}

		public static XamlConvertResult TryConvertValue(object value, Type targetType)
		{
			var result = ConvertValueImpl(value, targetType, out var exception);

			return exception == null ? new XamlConvertResult(result, null) : new XamlConvertResult(null, exception);
		}

		private class DummyTypeDescriptorContext : ITypeDescriptorContext
		{
			public static readonly ITypeDescriptorContext TypeDescriptorInstance = new DummyTypeDescriptorContext();

			private DummyTypeDescriptorContext()
			{
			}

			public IContainer Container => null;

			public object Instance => null;

			public PropertyDescriptor PropertyDescriptor => null;

			public object GetService(Type serviceType) => null;

			public bool OnComponentChanging() => false;

			public void OnComponentChanged()
			{
			}
		}
	}
}