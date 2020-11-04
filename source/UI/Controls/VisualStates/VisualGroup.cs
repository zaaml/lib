using System.Windows;
using System.Windows.Controls;

namespace Zaaml.UI.Controls.VisualStates
{
  internal abstract class VisualGroup
  {
    internal abstract void UpdateState(Control control, bool useTransitions);

    protected void GoToState(Control control, string stateName, bool useTransitions)
    {
      VisualStateManager.GoToState(control, stateName, useTransitions);
    }
  }
}