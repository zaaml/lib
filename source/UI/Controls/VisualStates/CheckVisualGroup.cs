using System.Windows.Controls;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.VisualStates
{
  internal class CheckVisualGroup : VisualGroup
  {
    public static readonly VisualGroup Instance = new CheckVisualGroup();

    private CheckVisualGroup()
    {
    }

    internal override void UpdateState(Control control, bool useTransitions)
    {
			var toggleButton = control as IToggleButton;
			if (toggleButton == null)
				return;

      var isChecked = toggleButton.IsChecked;

      if (isChecked == false)
		    GoToState(control, CommonVisualStates.Unchecked, useTransitions);
	    else if (isChecked == true)
		    GoToState(control, CommonVisualStates.Checked, useTransitions);
	    else
				GoToState(control, CommonVisualStates.Indeterminate, useTransitions);
    }
  }
}