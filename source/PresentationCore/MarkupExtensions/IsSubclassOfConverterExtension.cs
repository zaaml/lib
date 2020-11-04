// <copyright file="IsSubclassOfConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class IsSubclassOfConverterExtension : MarkupExtensionBase
  {
    #region Properties

    public bool Self { get; set; }
    public Type Type { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new IsSubclassOfConverter {Type = Type, Self = Self};
    }

    #endregion
  }
}