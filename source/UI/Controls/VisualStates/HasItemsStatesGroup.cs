// <copyright file="HasItemsStatesGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Interactivity;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.VisualStates
{
  internal class HasItemsStatesGroup : VisualGroup
  {
    #region Static Fields and Constants

    public static readonly VisualGroup Instance = new HasItemsStatesGroup();

    #endregion

    #region Ctors

    private HasItemsStatesGroup()
    {
    }

    #endregion

    #region  Methods

    internal override void UpdateState(Control control, bool useTransitions)
    {
      var itemsControl = control as IItemsControl;

      if (itemsControl == null)
        return;

      GoToState(control, itemsControl.HasItems ? CommonVisualStates.HasItems : CommonVisualStates.NoItems, useTransitions);
    }

    #endregion
  }
}