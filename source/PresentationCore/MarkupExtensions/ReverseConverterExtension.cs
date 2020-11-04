// <copyright file="ReverseConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class ReverseConverterExtension : MarkupExtensionBase
  {
    #region Properties

    public IValueConverter DirectConverter { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new ReverseConverter {DirectConverter = DirectConverter};
    }

    #endregion
  }
}