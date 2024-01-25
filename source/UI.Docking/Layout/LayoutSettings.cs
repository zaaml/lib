// <copyright file="LayoutSettings.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class LayoutSettings : DependencyObject
  {
    #region Properties

    public XElement Xml
    {
      get
      {
        var elementName = XName.Get(GetType().Name, XamlConstants.XamlZMNamespace);
        var layoutXml = new XElement(elementName, new XAttribute(XNamespace.Xmlns.GetName(XamlConstants.XamlZMPrefix), XamlConstants.XamlZMNamespace));

        foreach (var layoutKind in FullLayout.EnumerateLayoutKinds())
          BaseLayout.GetLayoutSerializer(FullLayout.GetLayoutType(layoutKind)).WriteProperties(this, layoutXml);

        return layoutXml;
      }
    }

    #endregion

    #region  Methods

    public static void ClearSettings(DependencyObject depObj, IEnumerable<DependencyProperty> dependencyProperties)
    {
      foreach (var depProperty in dependencyProperties)
        depObj.ClearValue(depProperty);
    }

    public void ClearSettings(IEnumerable<DependencyProperty> dependencyProperties)
    {
      ClearSettings(this, dependencyProperties);
    }

    public static void CopySettings(DependencyObject source, DependencyObject target, IEnumerable<DependencyProperty> dependencyProperties)
    {
      if (source == null || target == null || ReferenceEquals(source, target))
        return;

      foreach (var property in dependencyProperties)
      {
        if (source.HasLocalValue(property))
          target.SetValue(property, source.GetValue(property));
        else
          target.ClearValue(property);
      }
    }

    public static void MergeSettings(DependencyObject source, DependencyObject target, IEnumerable<DependencyProperty> dependencyProperties)
    {
      if (source == null || target == null || ReferenceEquals(source, target))
        return;

      foreach (var property in dependencyProperties)
      {
        if (source.HasLocalValue(property))
          target.SetValue(property, source.GetValue(property));
      }
    }

    public void LoadSettings(DependencyObject source, IEnumerable<DependencyProperty> dependencyProperties)
    {
      CopySettings(source, this, dependencyProperties);
    }

    public void StoreSettings(DependencyObject target, IEnumerable<DependencyProperty> dependencyProperties)
    {
      CopySettings(this, target, dependencyProperties);
    }

    public override string ToString()
    {
      return Xml.ToString();
    }

    #endregion
  }
}