// <copyright file="SelectionScopeExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Behaviors.Selectable
{
  internal class SelectionScopeExtension : MarkupExtensionBase
  {
    #region Properties

    public object Key { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return SelectableBehavior.GetOrCreateSelectionScopeByKey(Key);
    }

    #endregion
  }
}