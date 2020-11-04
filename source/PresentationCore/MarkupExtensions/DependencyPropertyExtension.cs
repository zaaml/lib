// <copyright file="DependencyPropertyExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class DependencyPropertyExtension : MarkupExtensionBase
  {
    #region Properties

    public string Property { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return DependencyPropertyUtils.ResolveAttachedDependencyProperty(Property, serviceProvider) ?? DependencyPropertyManager.UnresolvedDependencyProperty;
    }

    #endregion
  }
}