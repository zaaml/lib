// <copyright file="EffectiveValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.Core.Pools;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class EffectiveValue : IDependencyPropertyListener
  {
    #region Static Fields and Constants

    #endregion

    #region Fields

    private DependencyProperty _attachedProperty;
    private RuntimeSetter _currentSetter;
    private object _currentTargetLocalValue;
    private ushort _packedValue;
    private TargetValueListener _targetValueListener;

    #endregion

    #region Properties

    private EffectiveValueSource CurrentEffectiveValueSource
    {
      get => PackedDefinition.CurrentEffectiveValueSource.GetValue(_packedValue);
      set => PackedDefinition.CurrentEffectiveValueSource.SetValue(ref _packedValue, value);
    }

    public RuntimeSetter CurrentSetter
    {
      get => _currentSetter;
      private set
      {
        if (ReferenceEquals(_currentSetter, value))
          return;

        if (IsSetterSource(CurrentEffectiveValueSource))
          _currentSetter.OnDetached();

        _currentSetter = value;

        UpdateEffectiveValueSource(false);

        if (IsSetterSource(CurrentEffectiveValueSource))
          _currentSetter.OnAttached();

        SendValue();
      }
    }

    private PropertyValueSource CurrentValueSource
    {
      get => PackedDefinition.CurrentValueSource.GetValue(_packedValue);
      set => PackedDefinition.CurrentValueSource.SetValue(ref _packedValue, value);
    }

    internal bool HasSetter => _currentSetter != null;

    private bool IgnoreEvaluatedValueChange
    {
      get => PackedDefinition.IgnoreEvaluatedValueChange.GetValue(_packedValue);
      set => PackedDefinition.IgnoreEvaluatedValueChange.SetValue(ref _packedValue, value);
    }

    private bool IgnoreTargetValueChange
    {
      get => PackedDefinition.IgnoreTargetValueChange.GetValue(_packedValue);
      set => PackedDefinition.IgnoreTargetValueChange.SetValue(ref _packedValue, value);
    }

    private bool IsAttached
    {
      get => PackedDefinition.IsAttached.GetValue(_packedValue);
      set => PackedDefinition.IsAttached.SetValue(ref _packedValue, value);
    }

    private bool IsEvaluatedBinding
    {
      get => PackedDefinition.IsEvaluatedBinding.GetValue(_packedValue);
      set => PackedDefinition.IsEvaluatedBinding.SetValue(ref _packedValue, value);
    }

    private bool IsExpando
    {
      get => PackedDefinition.IsExpando.GetValue(_packedValue);
      set => PackedDefinition.IsExpando.SetValue(ref _packedValue, value);
    }

    private bool IsValueSetOnTarget
    {
      get => PackedDefinition.IsValueSetOnTarget.GetValue(_packedValue);
      set => PackedDefinition.IsValueSetOnTarget.SetValue(ref _packedValue, value);
    }

    public bool KeepAlive
    {
      get => PackedDefinition.ShouldLeave.GetValue(_packedValue);
      set => PackedDefinition.ShouldLeave.SetValue(ref _packedValue, value);
    }

    public DependencyProperty Property { get; private set; }

    public DependencyObject Target { get; private set; }

    #endregion

    #region  Methods

    public void AddSetter(RuntimeSetter setter)
    {
      CurrentSetter = AddSetterImpl(setter);
    }

    private RuntimeSetter AddSetterImpl(RuntimeSetter setter)
    {
      setter.NextSetter = null;

      var setterActualPriority = setter.ActualPriority;
      if (_currentSetter == null || _currentSetter.ActualPriority <= setterActualPriority)
      {
        setter.NextSetter = _currentSetter;
        return setter;
      }

      var prev = _currentSetter;
      var current = _currentSetter.NextSetter;

      while (current != null && setterActualPriority < current.ActualPriority)
      {
        prev = current;
        current = current.NextSetter;
      }

      prev.NextSetter = setter;
      setter.NextSetter = current;

      return CurrentSetter;
    }

    private static RuntimeSetter ApplyRuntimeSetter(DependencyObject target, DependencyProperty property, RuntimeSetter runtimeSetter)
    {
      runtimeSetter.Apply(EffectiveValueService.GetEffectiveValueImpl(target, property, true));

      return runtimeSetter;
    }

    private void AttachTargetValueListener()
    {
      if (_targetValueListener == null)
        _targetValueListener = new TargetValueListener(this);

      _targetValueListener.Attach();
    }

    private void DetachTargetValueListener()
    {
      _targetValueListener?.Detach();
    }

    public void Attach(DependencyObject target, DependencyProperty property)
    {
      IsAttached = true;

      Target = target;
      Property = property;

      var isExpando = property.IsExpando();
      IsExpando = isExpando;

      // Install target value listener
      IgnoreTargetValueChange = true;

      if (IsExpando)
        Target.GetDependencyPropertyService().AddExpandoPropertyListener(Property, this);
      else
        AttachTargetValueListener();

      IgnoreTargetValueChange = false;
      UpdateCurrentLocalValueAndSource();
    }

    private object ConvertTemplateBindingExpression(TemplateBindingExpression templateBindingExpression)
    {
#if SILVERLIGHT
      return templateBindingExpression == null ? null : DependencyProperty.UnsetValue;
#else
      return templateBindingExpression;
#endif
    }

    public static RuntimeSetter CreateAppliedSetter<T>(DependencyObject target, DependencyProperty property, object value, short priority, bool debug = false) where T : RuntimeSetter, new()
    {
      return ApplyRuntimeSetter(target, property, new T
      {
        Priority = RuntimeSetter.MergePriorityOrder(priority, 0),
        Value = value
      });
    }

    private void Detach()
    {
      IsAttached = false;

      if (IsExpando)
        Target.GetService<DependencyPropertyService>().RemoveExpandoListener(Property, this);
      else
        DetachTargetValueListener();

      try
      {
        if (CurrentEffectiveValueSource != EffectiveValueSource.Default)
        {
          if (IsLocalSource(CurrentValueSource) && _currentTargetLocalValue != DependencyProperty.UnsetValue)
            RestoreTargetValue(Property, _currentTargetLocalValue);
          else if (IsValueSetOnTarget)
            Target.ClearValue(Property);
        }
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }

      if (_attachedProperty != null)
      {
        Target.GetDependencyPropertyService().ReleaseServiceProperty(_attachedProperty);
        Target.ClearValue(_attachedProperty);
        _attachedProperty = null;
      }

      IsEvaluatedBinding = false;

      Target.GetService<EffectiveValueService>().RemoveEffectiveValue(this);
    }

    private void EnsureProperty(bool isHandlerSuspended)
    {
      if (_attachedProperty != null)
        return;

      if (isHandlerSuspended == false)
        IgnoreEvaluatedValueChange = true;

      _attachedProperty = Target.GetDependencyPropertyService().CaptureServiceProperty(Property.GetPropertyType(), this);
      Target.ClearValue(_attachedProperty);

      if (isHandlerSuspended == false)
        IgnoreEvaluatedValueChange = false;
    }

    public static EffectiveValue GetEffectiveValue(DependencyObject target, DependencyProperty property)
    {
      return EffectiveValueService.GetEffectiveValueImpl(target, property, true);
    }

    public static EffectiveValue GetEffectiveValue(DependencyObject dependencyObject, DependencyProperty dependencyProperty, bool create)
    {
      return EffectiveValueService.GetEffectiveValueImpl(dependencyObject, dependencyProperty, create);
    }


    private object GetTargetValue()
    {
      return Target.GetValue(Property);
    }

    public bool IsCurrentSetterValue(RuntimeSetter setter)
    {
      return ReferenceEquals(CurrentSetter, setter) && IsSetterSource(CurrentEffectiveValueSource);
    }

    private static bool IsLocalSource(PropertyValueSource valueSource)
    {
      return valueSource == PropertyValueSource.Local || valueSource == PropertyValueSource.LocalBinding;
    }

    private static bool IsSetterSource(EffectiveValueSource source) => source == EffectiveValueSource.StrongSetter || source == EffectiveValueSource.WeakSetter;

    private void LogError(RuntimeSetter runtimeSetter, Exception exception)
    {
      LogService.LogWarning($"Can not apply value for '{Property.GetName()}' property on target '{Target}':\n\t{exception.Message}");
    }

    private void LogError(Exception exception)
    {
      LogService.LogWarning($"Can not apply value for '{Property.GetName()}' property on target '{Target}':\n\t{exception.Message}");
    }

    public void OnRuntimeSetterPriorityChanged(RuntimeSetter setter)
    {
      if (CurrentSetter?.NextSetter == null)
        return;

      var current = _currentSetter;
      var priority = current.ActualPriority;
      var orderBroken = false;
      while (current != null)
      {
        current = current.NextSetter;
        if (current.ActualPriority <= priority)
        {
          priority = current.ActualPriority;
          continue;
        }

        orderBroken = true;
        break;
      }

      if (orderBroken == false)
        return;

      var currentSetter = _currentSetter;

      _currentSetter = RemoveSetterImpl(setter);
      _currentSetter = AddSetterImpl(setter);

      if (ReferenceEquals(_currentSetter, currentSetter) == false)
        UpdateEffectiveValue();
    }

    private PropertyValueSource GetTargetValueSource()
    {
      return Target.GetValueSource(Property);
    }

    private void OnTargetValueChanged(object oldValue, object newValue)
    {
      if (IsAttached == false || IgnoreTargetValueChange)
        return;

      var newSource = GetTargetValueSource();

      var noChange = Equals(oldValue, newValue);

      if (noChange && newSource == PropertyValueSource.Local)
        return;

      // Value changed by calling SetCurrentValue should not be overriden with Style setter
      if (newSource == PropertyValueSource.Default && newSource == CurrentValueSource)
        return;

      if (noChange && CurrentValueSource == newSource)
        return;

      UpdateCurrentLocalValueAndSource();
      UpdateEffectiveValue();
    }

    public void RemoveSetter(RuntimeSetter setter)
    {
      CurrentSetter = RemoveSetterImpl(setter);

      if (CurrentSetter == null && KeepAlive == false)
        Detach();
    }

    private RuntimeSetter RemoveSetterImpl(RuntimeSetter setter)
    {
      try
      {
        if (ReferenceEquals(_currentSetter, setter))
          return setter.NextSetter;

        var prev = _currentSetter;
        var current = _currentSetter.NextSetter;

        while (ReferenceEquals(current, setter) == false)
        {
          prev = current;
          current = current.NextSetter;
        }

        prev.NextSetter = current.NextSetter;

        return CurrentSetter;
      }
      finally
      {
        setter.NextSetter = null;
      }
    }

    private void RestoreTargetValue(DependencyProperty property, object value)
    {
      if (ReferenceEquals(Target.ReadLocalValue(property), value))
        return;

      Target.RestoreLocalValue(property,
        value as BindingExpression ??
        ConvertTemplateBindingExpression(value as TemplateBindingExpression) ?? value);
    }

    private void SendValue(RuntimeSetter effectiveSetter)
    {
      var value = effectiveSetter.ProvideValue(GetTargetValue());

      var exception = value as Exception;

      try
      {
        if (exception == null)
        {
          effectiveSetter.UpdateValue(SendValue(value));
          return;
        }
      }
      catch (Exception e)
      {
        exception = e;
      }

      LogError(effectiveSetter, exception);
    }

    private void SendValue()
    {
      switch (CurrentEffectiveValueSource)
      {
        case EffectiveValueSource.Default:
          SendValue(DependencyProperty.UnsetValue);
          break;
        case EffectiveValueSource.CurrentValue:
          SendValue(_currentTargetLocalValue);
          break;
        case EffectiveValueSource.StrongSetter:
        case EffectiveValueSource.WeakSetter:
          SendValue(CurrentSetter);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private object SendValue(object value)
    {
      try
      {
        IgnoreEvaluatedValueChange = true;

        var actualValue = value;

        var prevIsEvaluatedBinding = IsEvaluatedBinding;
        var bindingValue = value as Binding;

        var bindingExpression = value as BindingExpression;
        var templateBindingExpression = value as TemplateBindingExpression;

        object targetValue;

        if (bindingValue != null)
        {
          EnsureProperty(true);
          Target.SetBinding(_attachedProperty, bindingValue);
          actualValue = Target.ReadLocalValue(_attachedProperty);
          targetValue = Target.GetValue(_attachedProperty);
          IsEvaluatedBinding = true;
        }
        else if (bindingExpression != null || templateBindingExpression != null)
        {
          EnsureProperty(true);
          RestoreTargetValue(_attachedProperty, value);
          actualValue = Target.ReadLocalValue(_attachedProperty);
          targetValue = Target.GetValue(_attachedProperty);
          IsEvaluatedBinding = true;
        }
        else
        {
          targetValue = value;
          IsEvaluatedBinding = false;
        }

        if (IsEvaluatedBinding == false && prevIsEvaluatedBinding && _attachedProperty != null)
          Target.ClearValue(_attachedProperty);

        SetTargetValue(targetValue);

        return actualValue;
      }
      finally
      {
        IgnoreEvaluatedValueChange = false;
      }
    }

    private void ClearValueImpl()
    {
      try
      {
        Target.ClearValue(Property);
      }
      catch (Exception e)
      {
        LogError(e);
      }
      finally
      {
        IsValueSetOnTarget = false;
      }
    }

    private void SetValueImpl(object value)
    {
#if SILVERLIGHT
      Target.SetValue(Property, value);
      value.ResetInheritanceParent();
      IsValueSetOnTarget = true;
#else
      Target.SetCurrentValue(Property, value);

      // The problem with Inherited value. SetCurrentValue does not work in that case
      // var valueError = ReferenceEquals(Target.GetValue(Property), value) == false;
#endif
    }

    private void SetTargetValue(object value)
    {
      try
      {
        IgnoreTargetValueChange = true;

        var actualValue = IsEvaluatedBinding ? Target.ReadLocalValue(_attachedProperty) : value;
        var isUnset = actualValue.IsDependencyPropertyUnsetValue() || actualValue.IsUnset();

        if (isUnset)
          ClearValueImpl();
        else
          SetValueImpl(value);
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }
      finally
      {
        IgnoreTargetValueChange = false;
      }
    }

    private void UpdateCurrentLocalValueAndSource()
    {
      CurrentValueSource = GetTargetValueSource();
      _currentTargetLocalValue = IsLocalSource(CurrentValueSource) ? Target.ReadLocalValue(Property) : DependencyProperty.UnsetValue;
    }

    internal void UpdateEffectiveValue()
    {
      UpdateEffectiveValueSource(true);
      SendValue();
    }

    private void UpdateEffectiveValueSource(bool updateSetter)
    {
      var currentSource = CurrentEffectiveValueSource;

      if (CurrentSetter?.IsWeak == false)
        CurrentEffectiveValueSource = EffectiveValueSource.StrongSetter;
      else if (CurrentValueSource == PropertyValueSource.LocalBinding || CurrentValueSource == PropertyValueSource.Local)
        CurrentEffectiveValueSource = EffectiveValueSource.CurrentValue;
      else if (CurrentValueSource == PropertyValueSource.TemplatedParent)
        CurrentEffectiveValueSource = EffectiveValueSource.Default;
      else if (CurrentSetter != null)
        CurrentEffectiveValueSource = EffectiveValueSource.WeakSetter;
      else
        CurrentEffectiveValueSource = EffectiveValueSource.Default;

      if (currentSource == CurrentEffectiveValueSource || updateSetter == false) return;

      if (IsSetterSource(currentSource))
        CurrentSetter.OnDetached();

      if (IsSetterSource(CurrentEffectiveValueSource))
        CurrentSetter.OnAttached();
    }

    #endregion

    #region Interface Implementations

    #region IDependencyPropertyListener

    void IDependencyPropertyListener.OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
    {
      if (ReferenceEquals(dependencyProperty, _attachedProperty))
      {
        if (IsAttached == false || IgnoreEvaluatedValueChange)
          return;

        SetTargetValue(newValue);
      }

      if (ReferenceEquals(dependencyProperty, Property))
        OnTargetValueChanged(oldValue, newValue);
    }

    #endregion

    #endregion

    #region  Nested Types

    private sealed class EffectiveValueService : ServiceBase<DependencyObject>
    {
      #region Fields

      private readonly Dictionary<DependencyProperty, EffectiveValue> _attachedEffectiveValues = new Dictionary<DependencyProperty, EffectiveValue>();
      private MicroPool3<EffectiveValue> _detachedEffectiveValues;

#if !SILVERLIGHT
      // Workaround for issue:
      //https://connect.microsoft.com/VisualStudio/feedback/details/3136551/setcurrentvalue-does-not-work-on-property-with-inherited-value-wpf
      private static readonly DependencyProperty PreventInheritsAllProperty = DependencyProperty.RegisterAttached
        ("PreventInheritsAll", typeof(bool), typeof(EffectiveValueService), 
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
#endif

      #endregion

      #region  Methods

#if !SILVERLIGHT
      protected override void OnAttach()
      {
        base.OnAttach();
        Target.SetValue(PreventInheritsAllProperty, true);
      }
#endif
      private void AddEffectiveValue(EffectiveValue effectiveValue)
      {
        _attachedEffectiveValues[effectiveValue.Property] = effectiveValue;
      }

      private EffectiveValue GetEffectiveValueCore(DependencyObject target, DependencyProperty property, bool create = true)
      {
        EffectiveValue effectiveValue;
        if (_attachedEffectiveValues.TryGetValue(property, out effectiveValue))
          return effectiveValue;

        if (create == false)
          return null;

        effectiveValue = GetFreeEffectiveValue(target, property);
        AddEffectiveValue(effectiveValue);
        return effectiveValue;
      }

      public static EffectiveValue GetEffectiveValueImpl(DependencyObject target, DependencyProperty property, bool create)
      {
        var service = target.GetServiceOrCreateOrDefault(create, () => new EffectiveValueService());
        return service?.GetEffectiveValueCore(target, property, create);
      }

      private EffectiveValue GetFreeEffectiveValue(DependencyObject target, DependencyProperty property)
      {
        var freeEffectiveValue = _detachedEffectiveValues?.Mount() ?? new EffectiveValue();
        freeEffectiveValue.Attach(target, property);
        return freeEffectiveValue;
      }

      public void RemoveEffectiveValue(EffectiveValue effectiveValue)
      {
        _attachedEffectiveValues.Remove(effectiveValue.Property);

        if (_detachedEffectiveValues == null)
          _detachedEffectiveValues = new MicroPool3<EffectiveValue>();

        _detachedEffectiveValues.Release(effectiveValue);
      }

      #endregion
    }

    private class TargetValueListener : DependencyObject
    {
      #region Static Fields and Constants

      private static readonly DependencyProperty TargetValueListenerProperty = DPM.Register<object, TargetValueListener>
        ("TargetValueListener", e => e.OnTargetValueChanged);

      #endregion

      #region Fields

      private readonly EffectiveValue _effectiveValue;

      #endregion

      #region Ctors

      public TargetValueListener(EffectiveValue effectiveValue)
      {
        _effectiveValue = effectiveValue;
      }

      #endregion

      #region  Methods

      public void Attach()
      {
        this.SetBinding(TargetValueListenerProperty, new Binding
        {
          Path = new PropertyPath(_effectiveValue.Property),
          Source = _effectiveValue.Target,
          Mode = BindingMode.OneWay
        });
      }

      public void Detach()
      {
        ClearValue(TargetValueListenerProperty);
      }

      private void OnTargetValueChanged(object oldValue, object newValue)
      {
        _effectiveValue.OnTargetValueChanged(oldValue, newValue);
      }

      #endregion
    }

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsAttached;
      public static readonly PackedBoolItemDefinition IsEvaluatedBinding;
      public static readonly PackedBoolItemDefinition ShouldLeave;
      public static readonly PackedBoolItemDefinition IsValueSetOnTarget;
      public static readonly PackedBoolItemDefinition IgnoreTargetValueChange;
      public static readonly PackedBoolItemDefinition IgnoreEvaluatedValueChange;
      public static readonly PackedBoolItemDefinition IsExpando;
      public static readonly PackedEnumItemDefinition<EffectiveValueSource> CurrentEffectiveValueSource;
      public static readonly PackedEnumItemDefinition<PropertyValueSource> CurrentValueSource;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsAttached = allocator.AllocateBoolItem();
        IsEvaluatedBinding = allocator.AllocateBoolItem();
        ShouldLeave = allocator.AllocateBoolItem();
        IsValueSetOnTarget = allocator.AllocateBoolItem();
        IgnoreTargetValueChange = allocator.AllocateBoolItem();
        IgnoreEvaluatedValueChange = allocator.AllocateBoolItem();
        IsExpando = allocator.AllocateBoolItem();
        CurrentEffectiveValueSource = allocator.AllocateEnumItem<EffectiveValueSource>();
        CurrentValueSource = allocator.AllocateEnumItem<PropertyValueSource>();
      }

      #endregion
    }

    private enum EffectiveValueSource
    {
      Default = 0,
      WeakSetter = 1,
      CurrentValue = 2,
      StrongSetter = 3
    }

    #endregion
  }
}