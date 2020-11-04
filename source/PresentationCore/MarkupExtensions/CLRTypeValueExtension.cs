// <copyright file="CLRTypeValueExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class CLRTypeValueExtension<T> : MarkupExtensionBase
  {
    #region Properties

    public T Value { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return Value;
    }

    #endregion
  }

  //public class Null
}