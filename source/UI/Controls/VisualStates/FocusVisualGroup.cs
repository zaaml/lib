// <copyright file="FocusVisualGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Controls.VisualStates
{
  internal class FocusVisualGroup : VisualGroup
  {
    #region Static Fields and Constants

    public static readonly VisualGroup Instance = new FocusVisualGroup();

    #endregion

    #region Ctors

    private FocusVisualGroup()
    {
    }

    #endregion

    #region  Methods

    internal override void UpdateState(Control control, bool useTransitions)
    {
      var controlInterface = control as IControl;
      if (controlInterface == null)
        return;

      if (controlInterface.IsFocused && controlInterface.IsEnabled)
        GoToState(control, CommonVisualStates.Focused, useTransitions);
      else
        GoToState(control, CommonVisualStates.Unfocused, useTransitions);
    }

    #endregion
  }
}