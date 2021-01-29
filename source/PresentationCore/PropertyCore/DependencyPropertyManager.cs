// <copyright file="DependencyPropertyManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Reflection;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.PropertyCore
{
  internal static class DependencyPropertyManager
  {
    #region Static Fields and Constants

    private const BindingFlags DepPropFieldBindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
    private const BindingFlags NonPublicDepPropFieldBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
    private static readonly Dictionary<Type, List<DependencyPropertyInfo>> TypeToDPFieldCollection = new Dictionary<Type, List<DependencyPropertyInfo>>();
    private static readonly Dictionary<DependencyProperty, DependencyPropertyInfo> DP2DPInfo = new Dictionary<DependencyProperty, DependencyPropertyInfo>();
    private static readonly AppDomainObserver AppDomainObserver;

    private static readonly Dictionary<string, DependencyProperty> ExpandoProperties = new Dictionary<string, DependencyProperty>();

    internal static readonly DependencyProperty UnresolvedDependencyProperty = RegisterAttached("Unresolved", typeof(object), typeof(DependencyPropertyManager), new PropertyMetadata(null));

    #endregion

    #region Ctors

    static DependencyPropertyManager()
    {
      AppDomainObserver = new AppDomainObserver(RegisterAssemblyDependencyProperties);
    }

    #endregion

    #region  Methods

    private static IEnumerable<DependencyPropertyInfo> EnumerateDependencyPropertyInfos(Type type)
    {
      const BindingFlags dpFlags = BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public;

      return type.GetFields(dpFlags).Where(IsDPField).Select(fi => new DependencyPropertyInfo(fi)).Where(di => di.IsValid);
    }

    public static Type GetDeclaringType(this DependencyProperty dependencyProperty)
    {
      return dependencyProperty.GetDependencyPropertyInfo()?.DeclaringType;
    }

    public static string GetDeclaringTypeName(this DependencyProperty dependencyProperty)
    {
#if SILVERLIGHT
      return dependencyProperty.GetDependencyPropertyInfo()?.DeclaringType.Name ?? "Unknown";
#else
	    return dependencyProperty.OwnerType.Name;
#endif
    }

	  private static string CoercePropertyName(string name)
	  {
		  return name.Trim();
	  }

	  internal static DependencyProperty GetDependencyProperty(string name, Type ownerType, bool nonPublic)
		{
			return ownerType.GetField($"{CoercePropertyName(name)}Property", nonPublic ? NonPublicDepPropFieldBindingFlags : DepPropFieldBindingFlags)?.GetStaticValue<DependencyProperty>();
		}

		public static DependencyProperty GetDependencyProperty(string name, Type ownerType)
    {
      return ownerType.GetField($"{CoercePropertyName(name)}Property", DepPropFieldBindingFlags)?.GetStaticValue<DependencyProperty>();
    }

    public static DependencyProperty GetDependencyProperty(PropertyInfo propertyInfo)
    {
      return GetDependencyProperty(propertyInfo.Name, propertyInfo.DeclaringType);
    }

    public static DependencyPropertyInfo GetDependencyPropertyInfo(this DependencyProperty dependencyProperty)
    {
#if SILVERLIGHT
			AppDomainObserver.Update();
      return DP2DPInfo.GetValueOrDefault(dependencyProperty);
#else
      return DP2DPInfo.GetValueOrCreate(dependencyProperty, () => new DependencyPropertyInfo(dependencyProperty));
#endif
    }

    internal static DependencyProperty GetExpandoProperty(string name)
    {
      return GetExpandoPropertyImpl(name);
    }

    private static DependencyProperty GetExpandoPropertyImpl(string name)
    {
      return ExpandoProperties.GetValueOrCreate(name, n => RegisterExpandoProperty(name));
    }

    public static string GetName(this DependencyProperty dependencyProperty)
    {
#if SILVERLIGHT
	    return dependencyProperty.GetDependencyPropertyInfo()?.Name ?? "Unknown";
#else
	    return dependencyProperty.Name;
#endif
    }

    public static Type GetPropertyType(this DependencyProperty dependencyProperty)
    {
#if SILVERLIGHT
	    return dependencyProperty.GetDependencyPropertyInfo()?.PropertyType;
#else
	    return dependencyProperty.PropertyType;
#endif
    }

    public static string GetQualifiedName(this DependencyProperty dependencyProperty)
    {
      return string.Concat(dependencyProperty.GetDeclaringTypeName(), ".", dependencyProperty.GetName());
    }

    public static List<DependencyProperty> GetTypeDependencyProperties(Type type)
    {
      AppDomainObserver.Update();

      return TypeToDPFieldCollection.GetValueOrDefault(type, () => new List<DependencyPropertyInfo>()).Select(di => di.DependencyProperty).ToList();
    }

    public static List<DependencyPropertyInfo> GetTypeDependencyPropertyInfos(Type type)
    {
      AppDomainObserver.Update();

      return TypeToDPFieldCollection.GetValueOrDefault(type, () => new List<DependencyPropertyInfo>());
    }

    public static string GetTypeName(this DependencyProperty dependencyProperty)
    {
      return dependencyProperty.GetPropertyType()?.Name ?? "Unknown";
    }

    private static bool IsDPField(FieldInfo fieldInfo)
    {
      return fieldInfo.FieldType == typeof(DependencyProperty);
    }

    public static bool IsExpando(this DependencyProperty dependencyProperty)
    {
      return dependencyProperty.GetDependencyPropertyInfo()?.IsExpando ?? false;
    }

    private static void OnExpandoPropertyValueChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
    {
      depObj.GetService<DependencyPropertyService>()?.OnExpandoPropertyValueChanged(depObj, args.Property, args.OldValue, args.NewValue);
    }

    public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
      return RegisterImpl(name, propertyType, ownerType, typeMetadata);
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.DefaultCallback));
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.Callback(handlerFactory)));
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.Callback(handlerFactory)));
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.Callback(handlerFactory)));
    }


    internal static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.DefaultCallback));
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.Callback(handlerFactory)));
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.Callback(handlerFactory)));
    }

    internal static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty, TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.Callback(handlerFactory)));
    }

    private static void RegisterAssemblyDependencyProperties(IEnumerable<Assembly> assemblies)
    {
      foreach (var assembly in assemblies)
      {
#if !SILVERLIGHT
        if (assembly.GetReferencedAssemblies().Select(a => a.FullName).Contains(typeof(DependencyProperty).Assembly.GetName().FullName) == false)
          continue;
#endif
        foreach (var type in assembly.GetLoadableTypes().Where(t => t.IsGenericType == false && t.IsPublic))
        {
          var typeDP = EnumerateDependencyPropertyInfos(type).ToList();
          TypeToDPFieldCollection[type] = typeDP;
          foreach (var dpInfo in typeDP)
            RegisterDependencyPropertyInfo(dpInfo);
        }
      }
    }

    public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
      return RegisterAttachedImpl(name, propertyType, ownerType, typeMetadata);
    }


    internal static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, PropertyMetadata metadata)
    {
      return RegisterAttachedImpl(name, typeof(TProperty), ownerType, metadata);
    }

    internal static DependencyProperty RegisterAttached<TTarget, TProperty>(string name, Type ownerType, TProperty defaultValue = default(TProperty),
      Action<TTarget, TProperty, TProperty> handler = null)
      where TTarget : DependencyObject
    {
      return RegisterAttachedImpl(name, typeof(TProperty), ownerType, new PropertyMetadata(defaultValue, DPM.StaticCallback(handler)));
    }

    internal static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue = default(TProperty),
      Action<DependencyObject, TProperty, TProperty> handler = null)
    {
      return RegisterAttachedImpl(name, typeof(TProperty), ownerType, new PropertyMetadata(defaultValue, DPM.StaticCallback(handler)));
    }

    //internal static DependencyProperty RegisterAttached<TTarget, TProperty>(string name, Type ownerType, TProperty defaultValue = default(TProperty),
    //  Action<TTarget, ValueChangedEventArgs<TProperty>> handler = null) where TTarget : DependencyObject
    //{
    //  return RegisterAttachedImpl(name, typeof (TProperty), ownerType, new PropertyMetadata(defaultValue, DPM.StaticCallback(handler)));
    //}

    //internal static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue = default(TProperty),
    //  Action<DependencyObject, ValueChangedEventArgs<TProperty>> handler = null)
    //{
    //  return RegisterAttachedImpl(name, typeof (TProperty), ownerType, new PropertyMetadata(defaultValue, DPM.StaticCallback(handler)));
    //}

    internal static DependencyProperty RegisterAttached<TTarget, TProperty>(string name, Type ownerType, TProperty defaultValue = default(TProperty),
      Action<TTarget> handler = null)
      where TTarget : DependencyObject
    {
      return RegisterAttachedImpl(name, typeof(TProperty), ownerType, new PropertyMetadata(defaultValue, DPM.StaticCallback(handler)));
    }

    internal static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue = default(TProperty),
      Action<DependencyObject> handler = null)
    {
      return RegisterAttachedImpl(name, typeof(TProperty), ownerType, new PropertyMetadata(defaultValue, DPM.StaticCallback(handler)));
    }

    private static DependencyProperty RegisterAttachedImpl(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
      var dependencyProperty = DependencyProperty.RegisterAttached(name, propertyType, ownerType, typeMetadata);

      RegisterDependencyPropertyInfo(new DependencyPropertyInfo(dependencyProperty, name, ownerType, propertyType, true, false));

      return dependencyProperty;
    }

    public static DependencyPropertyKey RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
      return RegisterAttachedReadOnlyImpl(name, propertyType, ownerType, typeMetadata);
    }

    private static DependencyPropertyKey RegisterAttachedReadOnlyImpl(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
#if SILVERLIGHT
      return new DependencyPropertyKey(RegisterAttachedImpl(name, propertyType, ownerType, typeMetadata));
#else
      var dependencyPropertyKey = DependencyProperty.RegisterAttachedReadOnly(name, propertyType, ownerType, typeMetadata);

      RegisterDependencyPropertyInfo(new DependencyPropertyInfo(dependencyPropertyKey.DependencyProperty, name, ownerType, propertyType, true, false));

      return dependencyPropertyKey;
#endif
    }

    private static void RegisterDependencyPropertyInfo(DependencyPropertyInfo dpInfo)
    {
      if (DP2DPInfo.ContainsKey(dpInfo.DependencyProperty) == false)
        DP2DPInfo[dpInfo.DependencyProperty] = dpInfo;
    }

    private static DependencyProperty RegisterExpandoProperty(string name)
    {
      var propertyMetadata = new PropertyMetadata(null, OnExpandoPropertyValueChanged);
      var property = RegisterAttached(name, typeof(object), typeof(DependencyPropertyManager), propertyMetadata);

      var dpInfo = new DependencyPropertyInfo(property, name, typeof(DependencyPropertyManager), typeof(object), true, true);

      DP2DPInfo[property] = dpInfo;

      return property;
    }

    private static DependencyProperty RegisterImpl(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
      var dependencyProperty = DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);

      RegisterDependencyPropertyInfo(new DependencyPropertyInfo(dependencyProperty, name, ownerType, propertyType, false, false));

      return dependencyProperty;
    }

    public static DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
      return RegisterReadOnlyImpl(name, propertyType, ownerType, typeMetadata);
    }


    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.DefaultCallback));
    }

    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.Callback(handlerFactory)));
    }

    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.Callback(handlerFactory)));
    }

    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty), DPM.Callback(handlerFactory)));
    }


    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.DefaultCallback));
    }

    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.Callback(handlerFactory)));
    }

    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.Callback(handlerFactory)));
    }

    internal static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty, TProperty>> handlerFactory) where TOwner : DependencyObject
    {
      return RegisterReadOnlyImpl(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, DPM.Callback(handlerFactory)));
    }

    private static DependencyPropertyKey RegisterReadOnlyImpl(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
    {
#if SILVERLIGHT
      return new DependencyPropertyKey(RegisterImpl(name, propertyType, ownerType, typeMetadata));
#else
      var dependencyPropertyKey = DependencyProperty.RegisterReadOnly(name, propertyType, ownerType, typeMetadata);

      RegisterDependencyPropertyInfo(new DependencyPropertyInfo(dependencyPropertyKey.DependencyProperty, name, ownerType, propertyType, false, false));

      return dependencyPropertyKey;
#endif
    }

#endregion
  }
}