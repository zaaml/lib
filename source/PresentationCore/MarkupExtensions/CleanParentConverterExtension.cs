// <copyright file="CleanParentConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class CleanParentConverterExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return CleanParentConverter.Instance;
    }

    #endregion
  }
}