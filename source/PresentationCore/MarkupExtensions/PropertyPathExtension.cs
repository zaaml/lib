// <copyright file="PropertyPathExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class PropertyPathExtension : MarkupExtensionBase
  {
    #region Properties

    public object Property { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      var strProperty = Property as string;

      var dependencyProperty = strProperty != null ? DependencyPropertyUtils.ResolveAttachedDependencyProperty(strProperty, serviceProvider) : Property as DependencyProperty;
      return dependencyProperty != null ? new PropertyPath(dependencyProperty) : new PropertyPath(Property);
    }

    #endregion
  }
}