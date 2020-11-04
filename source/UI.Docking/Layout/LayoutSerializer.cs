// <copyright file="LayoutSerializer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Xml.Linq;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Docking
{
  internal abstract class LayoutSerializer
  {
    #region  Methods

    protected XName FormatProperty(Type layoutType, string layoutPropertyName)
    {
      return XName.Get($"{layoutType.Name}.{layoutPropertyName}", XamlConstants.XamlZMNamespace);
    }

    public abstract void WriteProperties(DependencyObject dependencyObject, XElement element);

    #endregion
  }
}