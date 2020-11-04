// <copyright file="IsNullConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class IsNullConverterExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return IsNullConverter.Instance;
    }

    #endregion
  }
}