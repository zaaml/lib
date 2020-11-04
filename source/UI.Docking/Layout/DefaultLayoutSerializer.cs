// <copyright file="DefaultLayoutSerializer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DefaultLayoutSerializer : LayoutSerializer
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Type, DefaultLayoutSerializer> DefaultSerializers = new Dictionary<Type, DefaultLayoutSerializer>();

    #endregion

    #region Fields

    private readonly Type _layoutType;

    #endregion

    #region Ctors

    private DefaultLayoutSerializer(Type layoutType)
    {
      _layoutType = layoutType;
    }

    #endregion

    #region  Methods

    public static LayoutSerializer FromType(Type layoutType)
    {
      return DefaultSerializers.GetValueOrCreate(layoutType, t => new DefaultLayoutSerializer(t));
    }

    public override void WriteProperties(DependencyObject dependencyObject, XElement element)
    {
      var layoutProperties = BaseLayout.GetLayoutProperties(_layoutType);

      foreach (var layoutProperty in layoutProperties)
      {
        if (BaseLayout.ShouldSerializeProperty(_layoutType, dependencyObject, layoutProperty) == false)
          continue;

        var value = dependencyObject.GetValue(layoutProperty);

        if (value == null)
          continue;

        var stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(stringValue))
          continue;

        var propertyName = FormatProperty(_layoutType, layoutProperty.GetName());

        element.Add(new XAttribute(propertyName, stringValue));
      }
    }

    #endregion
  }
}