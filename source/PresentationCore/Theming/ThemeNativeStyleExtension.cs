// <copyright file="ThemeNativeStyleExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using Binding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class ThemeNativeStyleExtension : BindingMarkupExtension
  {
    #region Properties

    protected override bool SupportNativeSetter => true;

    public Type TargetType { get; set; }

    #endregion

    #region  Methods

    protected internal override Binding GetBinding(IServiceProvider serviceProvider)
    {
      return ThemeManager.GetThemeStyle(TargetType ?? GetSafeTarget(serviceProvider)?.GetType()).StyleService.NativeStyleBinding;
    }

    #endregion
  }
}