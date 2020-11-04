// <copyright file="VisualStateManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Interactivity.VSM;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.PropertyCore;
using VisualState = System.Windows.VisualState;
using VisualStateGroup = System.Windows.VisualStateGroup;

#if SILVERLIGHT
using System.Windows.Controls;
#endif

namespace Zaaml.PresentationCore.Interactivity
{
  [ContentProperty(nameof(Groups))]
  public class VisualStateManager : System.Windows.VisualStateManager
  {
    public static readonly DependencyProperty GroupsProperty = DPM.Register<VisualStateGroupCollection, VisualStateManager>
      ("Groups");

    public static readonly DependencyProperty InstanceProperty = DPM.RegisterAttached<VisualStateManager, VisualStateManager>
      ("Instance", OnVisualStateManagerInstanceChanged);

    private static void OnVisualStateManagerInstanceChanged(DependencyObject depObj, VisualStateManager oldManager, VisualStateManager newManager)
    {
      var fre = depObj as FrameworkElement;
      if (fre == null)
        return;

      SetCustomVisualStateManager(fre, newManager);
      GetVisualStateGroups(fre)?.Clear();
    }

    public static void SetInstance(DependencyObject element, VisualStateManager value)
    {
      element.SetValue(InstanceProperty, value);
    }

    public static VisualStateManager GetInstance(DependencyObject element)
    {
      return (VisualStateManager) element.GetValue(InstanceProperty);
    }

    public VisualStateGroupCollection Groups
    {
      get => (VisualStateGroupCollection) GetValue(GroupsProperty);
      set => SetValue(GroupsProperty, value);
    }

    private static readonly object UseTransitionsTrue = new object();
    private static readonly object UseTransitionsFalse = new object();

    private object _useTransitionsStore;

#if SILVERLIGHT
    protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
    {
      var interactivityService = control.GetInteractivityService();
	    try
	    {
		    EnterStateChange(interactivityService, useTransitions);

        var goToStateCore = base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
	      var result = state != null && goToStateCore;

		    result |= interactivityService.GoToState(stateName, group, state, useTransitions);

	      return result;
	    }
	    catch
	    {
		    return false;
	    }
      finally
      {
        ExitStateChange(interactivityService);
      }
    }
#else

    protected override bool GoToStateCore(FrameworkElement control, FrameworkElement stateGroupsRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
    {
      var interactivityService = control.GetInteractivityService();

      try
      {
        EnterStateChange(interactivityService, useTransitions);

        // WPF bug: argument passed to this method actually could be null
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        var goToStateCore = state != null && base.GoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions);

        goToStateCore |= interactivityService.GoToState(stateName, group, state, useTransitions);

        return goToStateCore;
      }
      catch
      {
        return false;
      }
      finally
      {
        ExitStateChange(interactivityService);
      }
    }

#endif

    private static object GetUseTransitions(bool useTransitions)
    {
      return useTransitions ? UseTransitionsTrue : UseTransitionsFalse;
    }

    private static bool GetUseTransitions(object useTransitions)
    {
      return ReferenceEquals(useTransitions, UseTransitionsTrue);
    }

    private void EnterStateChange(IInteractivityService interactivityService, bool useTransitions)
    {
      // First entry
      if (_useTransitionsStore == null)
        _useTransitionsStore = GetUseTransitions(interactivityService.UseTransitions);
      else
      {
        // Reentry, save previous state to stack
        var stack = _useTransitionsStore as Stack<object>;

        // Wrap state store in stack
        if (stack == null)
        {
          stack = new Stack<object>();
          stack.Push(_useTransitionsStore);
        }
        stack.Push(interactivityService.UseTransitions);
      }

      interactivityService.UseTransitions = useTransitions;
    }


    private void ExitStateChange(IInteractivityService interactivityService)
    {
      var stack = _useTransitionsStore as Stack<object>;

      if (stack != null)
        interactivityService.UseTransitions = GetUseTransitions(stack.Pop());
      else
      {
        interactivityService.UseTransitions = GetUseTransitions(_useTransitionsStore);
        _useTransitionsStore = null;
      }
    }
  }

  public class VisualStateManagerExtension : MarkupExtensionBase
  {
    #region Properties

    public VisualStateGroupCollection Groups { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new VisualStateManager {Groups = Groups};
    }

    #endregion
  }
}