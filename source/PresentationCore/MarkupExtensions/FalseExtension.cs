// <copyright file="FalseExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class FalseExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return KnownBoxes.BoolFalse;
    }

    #endregion
  }
}