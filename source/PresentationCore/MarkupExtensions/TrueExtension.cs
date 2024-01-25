// <copyright file="TrueExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Runtime;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class TrueExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return BooleanBoxes.True;
    }

    #endregion
  }
}