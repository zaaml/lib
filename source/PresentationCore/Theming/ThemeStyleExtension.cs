// <copyright file="ThemeStyleExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class ThemeStyleExtension : MarkupExtensionBase
  {
    #region Properties

    public Type TargetType { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return TargetType != null ? ThemeManager.GetThemeStyle(TargetType) : null;
    }

    #endregion
  }
}
