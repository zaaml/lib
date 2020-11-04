// <copyright file="SetVisualStateAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>


using System.Windows.Controls;
using Zaaml.Core.Extensions;

#if !SILVERLIGHT
using System.Windows;
#endif

namespace Zaaml.PresentationCore.Interactivity
{
  public sealed class SetVisualStateAction : TargetTriggerActionBase
  {
    #region Properties

    public string VisualState { get; set; }

    public bool UseTransitions { get; set; }

    #endregion

    #region  Methods

    protected override InteractivityObject CreateInstance()
    {
      return new SetVisualStateAction();
    }

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var setVisualStateAction = (SetVisualStateAction)source;
      VisualState = setVisualStateAction.VisualState;
      UseTransitions = setVisualStateAction.UseTransitions;
    }

    protected override void InvokeCore()
    {
      if (VisualState.IsNullOrEmpty())
        return;

#if SILVERLIGHT
      var target = ActualTarget as Control;
#else
      var target = ActualTarget as FrameworkElement;
#endif
      if (target == null)
        return;

      System.Windows.VisualStateManager.GoToState(target, VisualState, UseTransitions);
    }

    #endregion
  }
}