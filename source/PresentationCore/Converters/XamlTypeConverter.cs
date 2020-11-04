// <copyright file="XamlTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Converters
{
  internal sealed class XamlTypeConverter : TypeConverter
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Type, XamlTemplate> TypeTemplates = new Dictionary<Type, XamlTemplate>();
    private static readonly Dictionary<Type, XamlTypeConverter> Converters = new Dictionary<Type, XamlTypeConverter>();

    #endregion

    #region Fields

    private readonly XamlTemplate _xamlTemplate;

    #endregion

    #region Ctors

    private XamlTypeConverter(Type type)
    {
      var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
      _xamlTemplate = TypeTemplates.GetValueOrCreate(underlyingType, () => new XamlTemplate(underlyingType));
    }

    #endregion

    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return false;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var strVal = value as string;
      if (strVal != null)
      {
        var siteStr = _xamlTemplate.Head + strVal + _xamlTemplate.Tail;
        var site = XamlUtils.Load<XamlConverterSite>(siteStr);

        if (site != null)
          return site.Value;
      }
      return base.ConvertFrom(context, culture, value);
    }

    public static T ConvertStringTo<T>(string strValue)
    {
      return (T) GetConverter<T>().ConvertFrom(strValue);
    }

    public static object ConvertStringTo(string strValue, Type type)
    {
      return GetConverter(type).ConvertFrom(strValue);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      throw new NotSupportedException();
    }

    public static XamlTypeConverter GetConverter(Type type)
    {
      return Converters.GetValueOrCreate(type, () => new XamlTypeConverter(type));
    }

    public static XamlTypeConverter GetConverter<T>()
    {
      return GetConverter(typeof(T));
    }

    #endregion

    #region  Nested Types

    private struct XamlTemplate
    {
      #region Fields

      public readonly string Head;
      public readonly string Tail;

      #endregion

      #region Ctors

      public XamlTemplate(Type type)
      {
        Head = $"<XamlConverterSite {GetClrNamespace(typeof(XamlConverterSite))}><{type.Name} {GetClrNamespace(type)}>";
        Tail = $"</{type.Name}>\n</XamlConverterSite>";
      }

      #endregion

      #region Methods

      private static string GetClrNamespace(Type type, string prefix = null)
      {
        var actualPrefix = prefix != null ? ":" + prefix : string.Empty;

        var assemblyName = type.Assembly.FullName.Split(',')[0];
        return $"xmlns{actualPrefix}=\"clr-namespace:{type.Namespace};assembly={assemblyName}\"";
      }

      #endregion
    }

    #endregion
  }
}