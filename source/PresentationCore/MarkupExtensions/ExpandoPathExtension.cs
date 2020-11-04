// <copyright file="ExpandoPathExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class ExpandoPathExtension : MarkupExtensionBase
  {
    #region Properties

    public string Property { get; set; }

    #endregion

    #region  Methods

    internal static PropertyPath CreatePropertyPath(string propertyName)
    {
      return new PropertyPath(DependencyPropertyManager.GetExpandoProperty(propertyName));
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return CreatePropertyPath(Property);
    }

    #endregion
  }
}