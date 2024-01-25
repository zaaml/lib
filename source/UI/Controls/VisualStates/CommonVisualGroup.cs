// <copyright file="CommonVisualGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.VisualStates
{
  internal class CommonVisualGroup : VisualGroup
  {
    #region Static Fields and Constants

    public static readonly VisualGroup Instance = new CommonVisualGroup();

    #endregion

    #region Ctors

    private CommonVisualGroup()
    {
    }

    #endregion

    #region  Methods

    internal override void UpdateState(Control control, bool useTransitions)
    {
      if (!control.IsEnabled)
        GoToState(control, CommonVisualStates.Disabled, useTransitions);
			else if (control is IButton { IsPressed: true })
				GoToState(control, CommonVisualStates.Pressed, useTransitions);
			else if (control is IControl { IsMouseOver: true })
				GoToState(control, CommonVisualStates.MouseOver, useTransitions);
      else if (control is IReadOnlyControl { IsReadOnly: true})
        GoToState(control, CommonVisualStates.ReadOnly, useTransitions);
      else
        GoToState(control, CommonVisualStates.Normal, useTransitions);
    }

    #endregion
  }
}