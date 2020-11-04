// <copyright file="MultiTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
  public sealed class MultiTrigger : DelayStateTriggerBase, IConditionalTrigger
  {
    #region Static Fields and Constants

    private static readonly uint DefaultPackedValue;

    #endregion

    #region Fields

    private ConditionCollection _conditions;

    #endregion

    #region Ctors

    static MultiTrigger()
    {
      RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

      PackedDefinition.Modifier.SetValue(ref DefaultPackedValue, ConditionLogicalOperator.And);
    }

    public MultiTrigger()
    {
      PackedValue |= DefaultPackedValue;
    }

    #endregion

    #region Properties

    private IEnumerable<ConditionBase> ActualConditions => _conditions ?? Enumerable.Empty<ConditionBase>();

    public ConditionCollection Conditions => _conditions ?? (_conditions = new ConditionCollection(this));

    public ConditionLogicalOperator LogicalOperator
    {
      get => PackedDefinition.Modifier.GetValue(PackedValue);
      set
      {
        PackedDefinition.Modifier.SetValue(ref PackedValue, value);
        UpdateTriggerState();
      }
    }

    #endregion

    #region  Methods

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var triggerSource = (MultiTrigger) source;

      LogicalOperator = triggerSource.LogicalOperator;
      _conditions = triggerSource._conditions?.DeepCloneCollection<ConditionCollection, ConditionBase>(this);
    }

    protected override InteractivityObject CreateInstance()
    {
      return new MultiTrigger();
    }

    internal override void DeinitializeTrigger(IInteractivityRoot root)
    {
      foreach (var condition in ActualConditions)
        condition.Unload(root);

      base.DeinitializeTrigger(root);
    }

    internal override void InitializeTrigger(IInteractivityRoot root)
    {
      base.InitializeTrigger(root);

      foreach (var condition in ActualConditions)
        condition.Load(root);
    }

    protected override void OnIsEnabledChanged()
    {
      foreach (var condition in ActualConditions)
        condition.IsEnabled = IsEnabled;

      base.OnIsEnabledChanged();
    }

    protected override TriggerState UpdateTriggerStateCore()
    {
      if (_conditions == null || _conditions.Count == 0)
        return TriggerState.Opened;

      switch (LogicalOperator)
      {
        case ConditionLogicalOperator.And:
          return _conditions.All(c => c.IsOpen) ? TriggerState.Opened : TriggerState.Closed;
        case ConditionLogicalOperator.Or:
          return _conditions.Any(c => c.IsOpen) ? TriggerState.Opened : TriggerState.Closed;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion

    #region Interface Implementations

    #region IConditionalTrigger

    void IConditionalTrigger.UpdateConditionalTrigger()
    {
      UpdateTriggerState();
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedEnumItemDefinition<ConditionLogicalOperator> Modifier;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = GetAllocator<MultiTrigger>();

        Modifier = allocator.AllocateEnumItem<ConditionLogicalOperator>();
      }

      #endregion
    }

    #endregion
  }

  internal interface IConditionalTrigger
  {
    #region  Methods

    void UpdateConditionalTrigger();

    #endregion
  }
}