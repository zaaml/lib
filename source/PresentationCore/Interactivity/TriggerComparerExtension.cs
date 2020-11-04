// <copyright file="TriggerComparerExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Interactivity
{
  public class TriggerComparerExtension : MarkupExtensionBase
  {
    #region Properties

    public ComparerOperator Operator { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return TriggerComparer.GetComparer(Operator);
    }

    #endregion
  }
}