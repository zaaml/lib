// <copyright file="ReflectionUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zaaml.Core.Reflection
{
  internal static class ReflectionUtil
  {
    #region Fields

    public delegate bool FindMemberAttributeFilter<in T>(T attribute) where T : Attribute;

    #endregion

    #region  Methods

    public static IEnumerable<Type> EnumerateBaseClasses(this Type type)
    {
      return EnumerateBaseClassesInt(type, false);
    }

    public static IEnumerable<Type> EnumerateBaseClassesAndSelf(this Type type)
    {
      return EnumerateBaseClassesInt(type, true);
    }

    private static IEnumerable<Type> EnumerateBaseClassesInt(Type type, bool self)
    {
      if (self)
        yield return type;

      var baseType = type;
      while ((baseType = baseType.BaseType) != null)
        yield return baseType;
    }

    public static FieldInfo FindField<T>(Type type, BindingFlags bindingFlags, FindMemberAttributeFilter<T> filter)
      where T : Attribute
    {
      return (from fi in type.GetFields(bindingFlags)
        let attrs = fi.GetCustomAttributes(typeof(T), true)
        where attrs.Length == 1 && filter(attrs[0] as T)
        select fi).FirstOrDefault();
    }

    public static List<PropertyInfo> FindProperties<T>(Type type, FindMemberAttributeFilter<T> filter)
      where T : Attribute
    {
      return (from pi in type.GetProperties()
        let attrs = pi.GetCustomAttributes(typeof(T), true)
        where attrs.Length == 1 && (filter == null || filter(attrs[0] as T))
        select pi).ToList();
    }

    public static T GetAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
    {
      return attributeProvider.GetAttributes<T>().FirstOrDefault();
    }

    public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
    {
      return attributeProvider.GetCustomAttributes(true).OfType<T>();
    }

    public static string[] GetEnumNames(this Type type)
    {
      return type.IsEnum ? type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToArray() : null;
    }

    public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
      try
      {
        return assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException e)
      {
        return e.Types.Where(t => t != null);
      }
    }

    public static T GetPropertyAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
    {
      var attrs = propertyInfo.GetCustomAttributes(typeof(T), true);
      return attrs.Length == 1 ? attrs[0] as T : null;
    }

    public static TValue GetStaticValue<TValue>(this FieldInfo fieldInfo)
    {
      return (TValue) fieldInfo.GetValue(null);
    }

    public static TValue GetValue<TValue>(this FieldInfo fieldInfo, object target)
    {
      return (TValue) fieldInfo.GetValue(target);
    }

    public static bool HasAnyAttribute<T1, T2>(this ICustomAttributeProvider attributeProvider, bool inherit)
      where T1 : Attribute
      where T2 : Attribute
    {
      foreach (var attribute in attributeProvider.GetCustomAttributes(inherit))
      {
        if (attribute is T1)
          return true;
        if (attribute is T2)
          return true;
      }

      return false;
    }

    public static bool HasAnyAttribute<T1, T2, T3>(this ICustomAttributeProvider attributeProvider, bool inherit)
      where T1 : Attribute
      where T2 : Attribute
      where T3 : Attribute
    {
      foreach (var attribute in attributeProvider.GetCustomAttributes(inherit))
      {
        if (attribute is T1)
          return true;
        if (attribute is T2)
          return true;
        if (attribute is T3)
          return true;
      }

      return false;
    }

    public static bool HasAnyAttribute<T1, T2, T3, T4>(this ICustomAttributeProvider attributeProvider, bool inherit)
      where T1 : Attribute
      where T2 : Attribute
      where T3 : Attribute
      where T4 : Attribute
    {
      foreach (var attribute in attributeProvider.GetCustomAttributes(inherit))
      {
        if (attribute is T1)
          return true;
        if (attribute is T2)
          return true;
        if (attribute is T3)
          return true;
        if (attribute is T4)
          return true;
      }

      return false;
    }

    public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : Attribute
    {
      return attributeProvider.GetCustomAttributes(inherit).OfType<T>().Any();
    }

    public static IEnumerable<Type> ReflectedTypes(this Type type)
    {
      for (var reflectedType = type.ReflectedType; reflectedType != null; reflectedType = reflectedType.ReflectedType)
        yield return reflectedType;
    }

    #endregion
  }
}