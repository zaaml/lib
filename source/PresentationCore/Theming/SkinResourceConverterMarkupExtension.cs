// <copyright file="SkinResourceConverterMarkupExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class SkinResourceConverterMarkupExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return SkinResourceConverter.Instance;
    }

    #endregion
  }
}