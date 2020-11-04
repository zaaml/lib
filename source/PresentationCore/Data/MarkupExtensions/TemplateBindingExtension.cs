// <copyright file="TemplateBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public sealed class TemplateBindingExtension : PathBindingBase
  {
    #region  Methods

    protected override void InitSource(NativeBinding binding)
    {
      binding.RelativeSource = XamlConstants.TemplatedParent;
    }

    #endregion
  }
}