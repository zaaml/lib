// <copyright file="Control.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Extensions;
using NativeControl = System.Windows.Controls.Control;
using NativeVisualStateManager = System.Windows.VisualStateManager;

namespace Zaaml.UI.Controls.Core
{
  public partial class Control : NativeControl, IControl, ILogicalOwner, ILogicalMentorOwner
	{
		private LogicalChildMentor<Control> _logicalChildMentor;

		#region Ctors

    public Control()
    {
      PlatformCtor();

      IsEnabledChanged += (sender, args) => OnIsEnabledChanged();
    }

    #endregion

    #region  Methods

    protected override Size MeasureOverride(Size availableSize)
    {
      return this.OnMeasureOverride(base.MeasureOverride, availableSize);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      UpdateVisualState(false);
    }

    protected virtual void OnIsEnabledChanged()
    {
      UpdateVisualState(true);
    }

    protected virtual void OnLoaded()
    {
    }

    protected virtual void OnUnloaded()
    {
    }

    partial void PlatformCtor();

    protected virtual void UpdateVisualState(bool useTransitions)
    {
      this.UpdateVisualGroups(useTransitions);
    }

    protected virtual bool GotoVisualState(string stateName, bool useTransitions)
    {
	    return NativeVisualStateManager.GoToState(this, stateName, useTransitions);
    }

		internal DependencyObject GetTemplateChildInternal(string name)
    {
      return GetTemplateChild(name);
    }

		protected override IEnumerator LogicalChildren => _logicalChildMentor == null ? base.LogicalChildren : _logicalChildMentor.GetLogicalChildren();

		private protected LogicalChildMentor LogicalChildMentor => _logicalChildMentor ??= LogicalChildMentor.Create(this);

    #endregion

    void ILogicalOwner.AddLogicalChild(object child)
    {
			LogicalChildMentor.AddLogicalChild(child);
		}

    void ILogicalMentorOwner.RemoveLogicalChild(object child)
    {
	    RemoveLogicalChild(child);
    }

    IEnumerator ILogicalMentorOwner.BaseLogicalChildren => base.LogicalChildren;

		void ILogicalMentorOwner.AddLogicalChild(object child)
    {
	    AddLogicalChild(child);
    }

    void ILogicalOwner.RemoveLogicalChild(object child)
    {
			LogicalChildMentor.RemoveLogicalChild(child);
		}

    IEnumerator ILogicalOwner.BaseLogicalChildren => base.LogicalChildren;
	}

  internal interface IZaamlControl
  {
    #region Properties

    IInteractivityService InteractivityService { get; }

    #endregion
  }
}