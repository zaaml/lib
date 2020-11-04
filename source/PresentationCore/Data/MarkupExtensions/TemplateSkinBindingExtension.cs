// <copyright file="TemplateSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public sealed class TemplateSkinBindingExtension : SkinBindingBaseExtension
  {
    #region Properties

    protected override RelativeSource Source => XamlConstants.TemplatedParent;

    #endregion
  }
}