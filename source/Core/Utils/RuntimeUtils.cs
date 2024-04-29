// <copyright file="RuntimeUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Reflection;
using Zaaml.Core.Collections;

namespace Zaaml.Core.Utils
{
	internal static class RuntimeUtils
	{
		private const BindingFlags TransformToDeclaringTypeBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private static readonly TwoWayDictionary<string, Type> TypeDictionary = new TwoWayDictionary<string, Type>();
		private static readonly Dictionary<Type, object> DefaultValuesDictionary = new Dictionary<Type, object>();

		static RuntimeUtils()
		{
			TypeDictionary.Add("bool", typeof(bool));
			TypeDictionary.Add("byte", typeof(byte));
			TypeDictionary.Add("sbyte", typeof(sbyte));
			TypeDictionary.Add("char", typeof(char));
			TypeDictionary.Add("decimal", typeof(decimal));
			TypeDictionary.Add("double", typeof(double));
			TypeDictionary.Add("float", typeof(float));
			TypeDictionary.Add("int", typeof(int));
			TypeDictionary.Add("uint", typeof(uint));
			TypeDictionary.Add("long", typeof(long));
			TypeDictionary.Add("ulong", typeof(ulong));
			TypeDictionary.Add("object", typeof(object));
			TypeDictionary.Add("short", typeof(short));
			TypeDictionary.Add("ushort", typeof(ushort));
			TypeDictionary.Add("string", typeof(string));

			foreach (var valueType in EnumeratePrimitiveValueTypes())
			{
				DefaultValuesDictionary[valueType] = Activator.CreateInstance(valueType);
			}
		}

		public static object CreateDefaultValue(Type type)
		{
			return type.IsValueType ? CreateDefaultValueTypeValue(type) : null;
		}

		private static object CreateDefaultValueTypeValue(Type type)
		{
			return DefaultValuesDictionary.TryGetValue(type, out var defaultValue) ? defaultValue : Activator.CreateInstance(type);
		}

		public static IEnumerable<Type> EnumeratePrimitiveTypes()
		{
			foreach (var primitiveType in EnumeratePrimitiveValueTypes())
				yield return primitiveType;

			yield return typeof(string);
		}

		public static IEnumerable<Type> EnumeratePrimitiveValueTypes()
		{
			yield return typeof(bool);
			yield return typeof(byte);
			yield return typeof(sbyte);
			yield return typeof(char);
			yield return typeof(decimal);
			yield return typeof(double);
			yield return typeof(float);
			yield return typeof(int);
			yield return typeof(uint);
			yield return typeof(long);
			yield return typeof(ulong);
			yield return typeof(object);
			yield return typeof(short);
			yield return typeof(ushort);
		}

		public static Type GetCSharpBuiltinType(string name)
		{
			return TypeDictionary.GetValueOrDefault(name);
		}

		public static string GetCSharpBuiltinTypeName(Type type)
		{
			return TypeDictionary.GetKeyOrDefault(type);
		}

		public static FieldInfo TransformToDeclaringTypeFieldInfo(FieldInfo fieldInfo)
		{
			return fieldInfo.DeclaringType != null && fieldInfo.DeclaringType != fieldInfo.ReflectedType
				? fieldInfo.DeclaringType.GetField(fieldInfo.Name, TransformToDeclaringTypeBindingFlags)
				: fieldInfo;
		}

		public static PropertyInfo TransformToDeclaringTypePropertyInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.DeclaringType != null && propertyInfo.DeclaringType != propertyInfo.ReflectedType
				? propertyInfo.DeclaringType.GetProperty(propertyInfo.Name, TransformToDeclaringTypeBindingFlags)
				: propertyInfo;
		}

		public static int GetTypeInheritanceDistance(Type baseType, Type type)
		{
			if (baseType.IsAssignableFrom(type) == false)
				return int.MaxValue;

			var distance = 0;

			while (type != null)
			{
				if (type == baseType)
					break;

				type = type.BaseType;
				distance++;
			}

			return distance;
		}
	}
}