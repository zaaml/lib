// <copyright file="RoundingConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class RoundingConverterExtension : MarkupExtensionBase
  {
    #region Properties

    public int Digits { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new RoundingConverter { Digits = Digits };
    }

    #endregion
  }
}