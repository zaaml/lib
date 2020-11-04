// <copyright file="SelfExpandoBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public sealed class SelfExpandoBindingExtension : ExpandoBindingBaseExtension
  {
    #region Properties

    protected override RelativeSource Source => XamlConstants.Self;

    #endregion
  }
}