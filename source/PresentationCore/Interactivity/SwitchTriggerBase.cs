// <copyright file="SwitchTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
  [ContentProperty("CaseTriggers")]
  public abstract class SwitchTriggerBase : TriggerBase
  {
    #region Fields

    private CaseTriggerCollection _caseTriggers;

    #endregion

    #region Properties

    private IEnumerable<CaseTriggerBase> ActualCaseTriggers => _caseTriggers ?? Enumerable.Empty<CaseTriggerBase>();

    protected abstract object ActualSourceValue { get; }

    public CaseTriggerCollection CaseTriggers => _caseTriggers ??= new CaseTriggerCollection(this);

    internal override IEnumerable<InteractivityObject> Children => base.Children.Concat(ActualCaseTriggers);

    #endregion

    #region  Methods

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var triggerSource = (SwitchTriggerBase) source;

      _caseTriggers = triggerSource._caseTriggers?.DeepCloneCollection<CaseTriggerCollection, CaseTriggerBase>(this);
    }

    internal override void LoadCore(IInteractivityRoot root)
    {
      base.LoadCore(root);

      foreach (var caseTrigger in ActualCaseTriggers)
        caseTrigger.Load(root);
    }

    protected override void OnIsEnabledChanged()
    {
      UpdateCaseTriggers();
      
      base.OnIsEnabledChanged();
    }

    internal override void UnloadCore(IInteractivityRoot root)
    {
      foreach (var caseTrigger in ActualCaseTriggers)
        caseTrigger.Unload(root);

      base.UnloadCore(root);
    }

    private void UpdateCaseTriggers()
    {
      foreach (var caseTrigger in ActualCaseTriggers)
        caseTrigger.IsEnabled = IsEnabled;
    }

    internal void UpdateTrigger()
    {
      UpdateTriggerState();
    }

    private void UpdateTriggerState()
    {
      var defaultCase = _caseTriggers?.DefaultCaseTrigger;
      var isAnyOpen = false;
      
      foreach (var trigger in ActualCaseTriggers.OfType<CaseTrigger>())
      {
				trigger.IsOpen = TriggerCompareUtil.Compare(ActualSourceValue, trigger.GetActualValue(ActualSourceValue?.GetType()), null);
        isAnyOpen |= trigger.IsOpen;
      }

      if (defaultCase != null)
        defaultCase.IsOpen = isAnyOpen == false;
    }

    #endregion
  }
}