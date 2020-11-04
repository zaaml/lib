// <copyright file="SelectableContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
  public class SelectableContentControl : ContentControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, SelectableContentControl>
      ("IsSelected", s => s.OnIsSelectedChanged);

    #endregion

    #region Properties

    public bool IsSelected
    {
      get => (bool) GetValue(IsSelectedProperty);
      set => SetValue(IsSelectedProperty, value);
    }

    #endregion

    #region  Methods

    private void OnIsSelectedChanged()
    {
      UpdateVisualState(true);
    }

    protected override void UpdateVisualState(bool useTransitions)
    {
      base.UpdateVisualState(useTransitions);

      var state = IsSelected ? (IsFocused ? CommonVisualStates.Selected : CommonVisualStates.SelectedInactive) : CommonVisualStates.Unselected;

      GotoVisualState(state, useTransitions);
    }

    #endregion
  }
}