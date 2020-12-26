// <copyright file="ConditionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
  public abstract class ConditionBase : InteractivityObject, IConditionalTrigger
  {
    #region Ctors

    static ConditionBase()
    {
      RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
    }

    #endregion

    #region Properties

    public bool Invert
    {
      get => PackedDefinition.Invert.GetValue(PackedValue);
      set
      {
        if (Invert == value)
          return;

        PackedDefinition.Invert.SetValue(ref PackedValue, value);

        UpdateConditionState();
      }
    }

    internal bool IsEnabled
    {
      get => PackedDefinition.IsEnabled.GetValue(PackedValue);
      set
      {
        if (IsEnabled == value)
          return;

        PackedDefinition.IsEnabled.SetValue(ref PackedValue, value);
        OnIsEnabledIntChanged();
      }
    }

    private bool IsInitializing
    {
      get => PackedDefinition.IsInitializing.GetValue(PackedValue);
      set => PackedDefinition.IsInitializing.SetValue(ref PackedValue, value);
    }

    public bool IsOpen
    {
      get => PackedDefinition.IsOpen.GetValue(PackedValue);
      private set
      {
        if (IsOpen == value)
          return;

        PackedDefinition.IsOpen.SetValue(ref PackedValue, value);
        OnIsOpenChanged();
      }
    }

    #endregion

    #region  Methods

    private bool ApplyInvert(bool value)
    {
      return Invert ? !value : value;
    }

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var conditionSource = (ConditionBase) source;
      
      PackedDefinition.Invert.SetValue(ref PackedValue, conditionSource.Invert);
    }

    internal virtual void Deinitialize(IInteractivityRoot root)
    {
    }

    internal virtual void Initialize(IInteractivityRoot root)
    {
    }

    internal sealed override void LoadCore(IInteractivityRoot root)
    {
      base.LoadCore(root);

      IsInitializing = true;

      Initialize(root);

      IsInitializing = false;

      UpdateConditionState();
    }

    protected virtual void OnIsEnabledIntChanged()
    {
    }

    private void OnIsOpenChanged()
    {
      UpdateConditionalTrigger();
      (Parent as IConditionalTrigger)?.UpdateConditionalTrigger();
    }

    internal sealed override void UnloadCore(IInteractivityRoot root)
    {
      Deinitialize(root);
      base.UnloadCore(root);
    }

    protected virtual void UpdateConditionalTrigger()
    {
    }

    protected void UpdateConditionState()
    {
      if (IsInitializing || IsLoaded == false)
        return;

      IsOpen = ApplyInvert(UpdateConditionStateCore());
    }

    protected abstract bool UpdateConditionStateCore();

    #endregion

    #region Interface Implementations

    #region IConditionalTrigger

    void IConditionalTrigger.UpdateConditionalTrigger()
    {
      UpdateConditionalTrigger();
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsEnabled;
      public static readonly PackedBoolItemDefinition IsOpen;
      public static readonly PackedBoolItemDefinition IsInitializing;
      public static readonly PackedBoolItemDefinition Invert;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = GetAllocator<ConditionBase>();

        IsEnabled = allocator.AllocateBoolItem();
        IsOpen = allocator.AllocateBoolItem();
        IsInitializing = allocator.AllocateBoolItem();
        Invert = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }
}