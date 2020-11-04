// <copyright file="ValueResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  [Flags]
  internal enum ValueKind
  {
    Unspecified = 31,
    TemplateExpandoPath = 0,
    SelfExpandoPath = 1,
    ThemeResourcePath = 2,
    SkinPath = 4,
    TemplateSkinPath = 8,
    ValuePath = 15,
    Explicit = 16,
    Resolved = 32,
    Inherited = Resolved | Unspecified
  }

  internal interface IValueSetter
  {
    #region Properties

    ValueKind ValueKind { get; set; }

    object ValueStore { get; set; }

    #endregion
  }

  internal static class ValueResolver
  {
    #region  Methods

    public static void CopyFrom(IValueSetter target, IValueSetter source)
    {
      target.ValueKind = source.ValueKind;
      target.ValueStore = source.ValueStore;
    }

    private static object GetOriginalValue(IValueSetter setter)
    {
      var valueStore = setter.ValueStore;

      if (IsResolvedValueProvider(setter.ValueKind) == false)
        return valueStore;

      return valueStore is ISetterValueProvider valueProvider ? valueProvider.OriginalValue : valueStore;
    }

    public static object GetValue(IValueSetter setter)
    {
      var valueKind = setter.ValueKind & ValueKind.Unspecified;

      return valueKind == ValueKind.Explicit ? GetOriginalValue(setter) : null;
    }

    public static string GetValuePath(IValueSetter setter)
    {
      var valueKind = setter.ValueKind & ValueKind.Unspecified;

      if (valueKind != ValueKind.Explicit)
        return (string) GetOriginalValue(setter);

      return null;
    }

    public static ValuePathSource GetValuePathSource(IValueSetter setter)
    {
      var valueKind = setter.ValueKind & ValueKind.Unspecified;
      
      switch (valueKind)
      {
        case ValueKind.TemplateExpandoPath:
          return ValuePathSource.TemplateExpando;
        case ValueKind.SelfExpandoPath:
          return ValuePathSource.Expando;
        case ValueKind.SkinPath:
          return ValuePathSource.Skin;
        case ValueKind.TemplateSkinPath:
	        return ValuePathSource.TemplateSkin;
        case ValueKind.ThemeResourcePath:
          return ValuePathSource.ThemeResource;
        default:
          return ValuePathSource.ThemeResource;
      }
    }

    private static bool IsResolvedValueProvider(ValueKind valueKind)
    {
      return (valueKind & ValueKind.Resolved) != 0;
    }

    public static bool IsSpecified(IValueSetter setter)
    {
      return (setter.ValueKind & ValueKind.Unspecified) != ValueKind.Unspecified;
    }

    public static bool IsSpecifiedValuePath(IValueSetter setter)
    {
      var valueKind = setter.ValueKind & ValueKind.Unspecified;

      return valueKind != ValueKind.Unspecified && valueKind != ValueKind.ValuePath;
    }

    public static ISetterValueProvider ResolveValueProvider(IValueSetter setter)
    {
      var valueProvider = setter.ValueStore as ISetterValueProvider;

      if (valueProvider != null)
        return valueProvider;

      var valueKind = setter.ValueKind;

      if (IsResolvedValueProvider(valueKind))
        return null;

      try
      {
        if (valueKind == ValueKind.ThemeResourcePath)
        {
          var actualValuePath = GetValuePath(setter);

          if (string.IsNullOrEmpty(actualValuePath) == false)
          {
            valueProvider = ThemeManager.GetThemeReference(actualValuePath);
            setter.ValueStore = valueProvider;

            return valueProvider;
          }
        }

        if (valueKind == ValueKind.Explicit)
        {
          var actualValue = GetValue(setter);
          var value = actualValue;

          if (value is ThemeResourceExtension themeResourceExtension)
            valueProvider = themeResourceExtension;
        }
      }
      finally
      {
        if (valueProvider != null)
          setter.ValueStore = valueProvider;

        setter.ValueKind |= ValueKind.Resolved;
      }

      return valueProvider;
    }

    public static void SetValue(IValueSetter setter, object value)
    {
      UnresolveValueProvider(setter);

      setter.ValueStore = value;
      setter.ValueKind = ValueKind.Explicit;
    }

    public static void SetValuePath(IValueSetter setter, string value)
    {
      UnresolveValueProvider(setter);

      var valueKind = setter.ValueKind & ValueKind.Unspecified;

      if (valueKind != ValueKind.SelfExpandoPath
          && valueKind != ValueKind.TemplateExpandoPath
          && valueKind != ValueKind.ThemeResourcePath
          && valueKind != ValueKind.SkinPath
          && valueKind != ValueKind.TemplateSkinPath)
        setter.ValueKind = ValueKind.ValuePath;

      setter.ValueStore = value;
    }

    public static void SetValuePathSource(IValueSetter setter, ValuePathSource value)
    {
      UnresolveValueProvider(setter);

      if (value == ValuePathSource.ThemeResource)
        setter.ValueKind = ValueKind.ThemeResourcePath;
      else if (value == ValuePathSource.Expando)
        setter.ValueKind = ValueKind.SelfExpandoPath;
      else if (value == ValuePathSource.TemplateExpando)
        setter.ValueKind = ValueKind.TemplateExpandoPath;
      else if (value == ValuePathSource.Skin)
        setter.ValueKind = ValueKind.SkinPath;
			else if (value == ValuePathSource.TemplateSkin)
	      setter.ValueKind = ValueKind.TemplateSkinPath;
      else
        setter.ValueKind = ValueKind.ThemeResourcePath;
    }

    public static void UnresolveValueProvider(IValueSetter setter)
    {
      var valueKind = setter.ValueKind;

      if (IsResolvedValueProvider(valueKind) == false)
        return;

      setter.ValueKind = valueKind & ValueKind.Unspecified;

      var valueStore = setter.ValueStore;

      if (valueStore is ISetterValueProvider valueProvider)
        setter.ValueStore = valueProvider.OriginalValue;
    }

    #endregion
  }
}