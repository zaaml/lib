// <copyright file="FlexDefinitionSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Panels.Flexible
{
  public abstract class FlexDefinitionSelector : InheritanceContextObject
  {
    #region Fields

    public event EventHandler SelectorChanged;

    #endregion

    #region  Methods

    protected virtual void OnSelectorChanged()
    {
      SelectorChanged?.Invoke(this, EventArgs.Empty);
    }

    public abstract FlexDefinition Select(Panel panel, UIElement child);

    #endregion
  }
}