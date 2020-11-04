using System.Windows.Controls;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.PresentationCore.Interactivity;

namespace Zaaml.UI.Controls.VisualStates
{
  internal class SelectionStatesGroup : VisualGroup
  {
    public static readonly VisualGroup Instance = new SelectionStatesGroup();

    private SelectionStatesGroup()
    {
    }

    internal override void UpdateState(Control control, bool useTransitions)
    {
      var selectionStateControl = control as ISelectionStateControl;
      var activeStateControl = control as IActiveStateControl;

      if (selectionStateControl == null)
        return;

      if (selectionStateControl.IsSelected)
        GoToState(control, activeStateControl == null || activeStateControl.IsActive ? CommonVisualStates.Selected : CommonVisualStates.SelectedInactive, useTransitions);
      else
        GoToState(control, CommonVisualStates.Unselected, useTransitions);
    }
  }
}