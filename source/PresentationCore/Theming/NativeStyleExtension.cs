// <copyright file="NativeStyleExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Markup;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using Binding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Theming
{
  [ContentProperty(nameof(Style))]
  public sealed class NativeStyleExtension : BindingMarkupExtension
  {
    #region Properties

    public StyleBase Style { get; set; }

    protected override bool SupportNativeSetter => true;

    #endregion

    #region  Methods

    protected internal override Binding GetBinding(IServiceProvider serviceProvider)
    {
      return Style?.StyleService.NativeStyleBinding;
    }

    protected override object ProvideValueCore(object target, object targetProperty, IServiceProvider serviceProvider)
    {
      if (Style == null)
        return null;

      return Style.IsThemeBased ? base.ProvideValueCore(target, targetProperty, serviceProvider) : Style.StyleService.NativeStyle;
    }

    #endregion
  }
}