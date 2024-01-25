// <copyright file="DependencyPropertyInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Reflection;

namespace Zaaml.PresentationCore.PropertyCore
{
  [DebuggerDisplay("{DeclaringTypeName}.{Name}")]
  internal class DependencyPropertyInfo
  {
    #region Ctors

    internal DependencyPropertyInfo(DependencyProperty dependencyProperty, string name, Type declaringType, Type propertyType, bool isAttached, bool isExpando)
    {
      DependencyProperty = dependencyProperty;
      Name = name;
      DeclaringType = declaringType;
      PropertyType = propertyType;
      IsAttached = isAttached;
      IsExpando = isExpando;
    }

    internal DependencyPropertyInfo(DependencyPropertyKey dependencyPropertyKey, DependencyProperty dependencyProperty, string name, Type declaringType, Type propertyType, bool isAttached, bool isExpando)
    {
	    DependencyPropertyKey = dependencyPropertyKey;
	    DependencyProperty = dependencyProperty;
      Name = name;
      DeclaringType = declaringType;
      PropertyType = propertyType;
      IsAttached = isAttached;
      IsExpando = isExpando;
    }

    internal DependencyPropertyInfo(FieldInfo fieldInfo)
    {
      try
      {
        DependencyProperty = fieldInfo.GetStaticValue<DependencyProperty>();
        var fiName = fieldInfo.Name;
        Name = fiName.EndsWith("Property") ? fiName.Substring(0, fiName.Length - 8) : null;
        DeclaringType = fieldInfo.DeclaringType;
        var reflectedType = fieldInfo.ReflectedType;
        var propertyInfo = reflectedType.GetProperty(Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var attachedGetter = reflectedType.GetMethod("Get" + Name);
        PropertyType = propertyInfo?.PropertyType ?? attachedGetter?.ReturnType;
        IsAttached = propertyInfo == null;
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }
    }

#if !SILVERLIGHT
    internal DependencyPropertyInfo(DependencyProperty dependencyProperty)
    {
      DependencyProperty = dependencyProperty;
      Name = dependencyProperty.Name;
      PropertyType = dependencyProperty.PropertyType;
      DeclaringType = dependencyProperty.OwnerType;
    }
#endif

    #endregion

    #region Properties

    public Type DeclaringType { get; }

    // ReSharper disable once UnusedMember.Local
    private string DeclaringTypeName => DeclaringType.Name;

    public DependencyPropertyKey DependencyPropertyKey { get; }
    public DependencyProperty DependencyProperty { get; }
    internal bool IsAttached { get; private set; }
    public bool IsExpando { get; set; }

    internal bool IsValid => DependencyProperty != null && Name != null && DeclaringType != null && PropertyType != null;

    public string Name { get; }
    public Type PropertyType { get; }

    #endregion

    #region  Methods

    public override string ToString()
    {
      return Name;
    }

    #endregion
  }
}