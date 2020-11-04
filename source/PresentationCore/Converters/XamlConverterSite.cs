// <copyright file="XamlConverterSite.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Converters
{
  [ContentProperty("Value")]
  public sealed class XamlConverterSite : DependencyObject
  {
    #region Properties

    public object Value { get; set; }

    #endregion
  }
}