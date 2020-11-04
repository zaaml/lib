// <copyright file="ActionTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
  [ContentProperty("Actions")]
  public abstract class ActionTriggerBase : TriggerBase
  {
    #region Fields

    private TriggerActionCollection _actions;
    private DelayAction _delayTrigger;

    #endregion

    #region Properties

    public TriggerActionCollection Actions => _actions ?? (_actions = new TriggerActionCollection(this));

    protected IEnumerable<TriggerActionBase> ActualActions => _actions ?? Enumerable.Empty<TriggerActionBase>();

    private DelayAction ActualDelayAction => _delayTrigger ?? (_delayTrigger = new DelayAction(InvokeCore, TimeSpan.Zero));

    internal sealed override IEnumerable<InteractivityObject> Children => base.Children.Concat(ActualActions);

    public Duration Delay
    {
      get => _delayTrigger?.Delay ?? default(Duration);
      set
      {
        if (Delay == value)
          return;

        ActualDelayAction.Delay = value.HasTimeSpan ? value.TimeSpan : TimeSpan.Zero;
      }
    }

    #endregion

    #region  Methods

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var sourceTrigger = (ActionTriggerBase) source;
      _actions = sourceTrigger._actions?.DeepCloneCollection<TriggerActionCollection, TriggerActionBase>(this);

      Delay = sourceTrigger.Delay;
    }

    protected void Invoke()
    {
      if (IsLoaded == false) return;

      if (_delayTrigger == null || _delayTrigger.Delay.Equals(TimeSpan.Zero))
        InvokeCore();
      else
        _delayTrigger.Invoke();
    }

    protected virtual void InvokeCore()
    {
      foreach (var action in ActualActions)
        action.Invoke();
    }

    internal override void LoadCore(IInteractivityRoot root)
    {
      foreach (var action in ActualActions)
        action.Load(root);

      base.LoadCore(root);
    }

    internal override void UnloadCore(IInteractivityRoot root)
    {
      foreach (var action in ActualActions)
        action.Unload(root);

      base.UnloadCore(root);
    }

    #endregion
  }
}