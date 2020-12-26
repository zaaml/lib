// <copyright file="PropertyValueSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Animation;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  [DebuggerDisplay("{" + nameof(DebugView) + "}")]
  public abstract class PropertyValueSetter : SetterBase, IInteractivityTargetSubject, IValueSetter, IServiceProvider, IPropertySubject, IProvideValueTarget
  {
    #region Static Fields and Constants

    private static readonly uint DefaultPackedValue;

    #endregion

    #region Fields

    private uint _priorityStore;
    private object _propertyStore;
    private object _runtimeTransitionStore = Unset.Value;
    private object _valueStore;

    #endregion

    #region Ctors

    static PropertyValueSetter()
    {
      RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

      PackedDefinition.SubjectKind.SetValue(ref DefaultPackedValue, SubjectKind.Unspecified);
      PackedDefinition.PropertyKind.SetValue(ref DefaultPackedValue, PropertyKind.Unspecified);
      PackedDefinition.ValueKind.SetValue(ref DefaultPackedValue, ValueKind.Unspecified);
    }

    protected PropertyValueSetter()
    {
      PackedValue |= DefaultPackedValue;
    }

    #endregion

    #region Properties

    protected short ActualPriority => FindSpecified(GetPrioritySpecified).Priority;

    protected DependencyProperty ActualProperty => PropertyResolver.ResolveProperty(FindSpecified(PropertyResolver.IsSpecified));

    protected Type ActualPropertyType => ActualProperty?.GetPropertyType();

    protected DependencyObject ActualTarget => SubjectResolver.ResolveSubject(FindSpecified(SubjectResolver.IsSpecified));

    internal ThemeResourceKey ActualThemeResourceKey
    {
      get
      {
        var valuePath = ThemeResourceKey.Builder;

        BuildActualValuePath(valuePath);

        return valuePath.Count == 0 ? ThemeResourceKey.Empty : new ThemeResourceKey(valuePath);
      }
    }

    protected Transition ActualTransition => FindSpecified(GetTransitionSpecified).Transition;

    protected object ActualValue => ValueResolver.GetValue(FindSpecified(ValueResolver.IsSpecified));

    protected string ActualValuePath => ActualThemeResourceKey.IsEmpty ? null : ActualThemeResourceKey.Key;

    protected ValuePathSource ActualValuePathSource => ValueResolver.GetValuePathSource(FindSpecified(ValueResolver.IsSpecifiedValuePath));

    protected string ActualVisualState => FindSpecified(GetVisualStateSpecified).VisualState;

    private string DebugView
    {
      get
      {
        var sb = new StringBuilder();

        sb.Append(GetType().Name);
        sb.Append(' ');
        if (ExpandoProperty != null)
        {
          sb.AppendFormat($"ExpandoProperty=\"{ExpandoProperty}\"");
          sb.Append(' ');
        }
        else if (Property != null)
        {
          var dependencyProperty = Property as DependencyProperty;
          sb.AppendFormat($"Property=\"{dependencyProperty?.GetName() ?? Property}\"");
          sb.Append(' ');
        }

        if (ValuePath != null)
        {
          sb.AppendFormat($"ValuePath=\"{ValuePath}\"");
          sb.Append(' ');
        }

        if (VisualState != null)
        {
          sb.AppendFormat($"VisualState=\"{VisualState}\"");
          sb.Append(' ');
        }

        if (IsPrioritySet)
        {
          sb.AppendFormat($"Priority=\"{Priority}\"");
          sb.Append(' ');
        }

        return sb.ToString().Trim();
      }
    }

    internal override DependencyProperty DependencyProperty => PropertyResolver.GetServiceProperty(this);

    [ApiProperty]
    public string ExpandoProperty
    {
      get => PropertyResolver.GetExpandoProperty(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldProperty = isAppliedOrQueried ? ActualProperty : null;

          PropertyResolver.SetExpandoProperty(this, value);

          if (isAppliedOrQueried == false) 
	          return;

          var newProperty = ActualProperty;

          if (ReferenceEquals(oldProperty, newProperty) == false)
            OnActualPropertyChanged(oldProperty, newProperty);
        }
        finally
        {
          OnApiPropertyChanged(nameof(ExpandoProperty));
        }
      }
    }

    internal override ushort Index
    {
      get => PackedPriorityDefinition.Index.GetValue(_priorityStore);
      set
      {
        var isAppliedOrQueried = IsAppliedOrQueried;

        var oldPriority = isAppliedOrQueried ? ActualPriority : (short) 0;

        PackedPriorityDefinition.Index.SetValue(ref _priorityStore, value);

        if (isAppliedOrQueried == false) 
	        return;

        var newPriority = ActualPriority;

        if (oldPriority != newPriority)
          OnActualPriorityChanged(oldPriority, newPriority);
      }
    }

    protected bool IsPrioritySet
    {
      get => PackedDefinition.IsPrioritySet.GetValue(PackedValue);
      private set => PackedDefinition.IsPrioritySet.SetValue(ref PackedValue, value);
    }

    protected bool IsTransitionSet
    {
      get => PackedDefinition.IsTransitionSet.GetValue(PackedValue);
      private set => PackedDefinition.IsTransitionSet.SetValue(ref PackedValue, value);
    }

    protected bool IsVisualStateSet => VisualStateIndex > 0;

    internal override DependencyProperty MergeDependencyProperty
    {
      get
      {
        var dependencyProperty = DependencyProperty;

        if (dependencyProperty == null)
          return null;

        var actualVisualState = ActualVisualState;

        return string.IsNullOrEmpty(actualVisualState) ? dependencyProperty : DependencyPropertyProxyManager.GetDependencyProperty(string.Concat(dependencyProperty.GetQualifiedName(), ".", actualVisualState));
      }
    }

    [ApiProperty]
    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public short Priority
    {
      get => PackedPriorityDefinition.Priority.GetValue(_priorityStore);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldPriority = isAppliedOrQueried ? ActualPriority : (short) 0;

          PackedPriorityDefinition.Priority.SetValue(ref _priorityStore, value);
          IsPrioritySet = true;

          if (isAppliedOrQueried == false)
	          return;

          var newPriority = ActualPriority;
          if (oldPriority != newPriority)
            OnActualPriorityChanged(oldPriority, newPriority);
        }
        finally
        {
          OnApiPropertyChanged(nameof(Priority));
        }
      }
    }

    [ApiProperty]
    public object Property
    {
      get => PropertyResolver.GetProperty(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldProperty = isAppliedOrQueried ? ActualProperty : null;

          PropertyResolver.SetProperty(this, value);

          if (isAppliedOrQueried == false) 
	          return;

          var newProperty = ActualProperty;
          if (ReferenceEquals(oldProperty, newProperty) == false)
            OnActualPropertyChanged(oldProperty, newProperty);
        }
        finally
        {
          OnApiPropertyChanged(nameof(Property));
        }
      }
    }

    private PropertyKind PropertyKind
    {
      get => PackedDefinition.PropertyKind.GetValue(PackedValue);
      set => PackedDefinition.PropertyKind.SetValue(ref PackedValue, value);
    }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    protected object RuntimeTransitionStore
    {
      get => _runtimeTransitionStore;
      set => _runtimeTransitionStore = value;
    }

    private SubjectKind SubjectKind
    {
      get => PackedDefinition.SubjectKind.GetValue(PackedValue);
      set => PackedDefinition.SubjectKind.SetValue(ref PackedValue, value);
    }

    [ApiProperty]
    public object Target
    {
      get => SubjectResolver.GetExplicitSubject(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldTarget = isAppliedOrQueried ? ActualTarget : null;

          SubjectResolver.SetExplicitSubject(this, value);

          if (isAppliedOrQueried == false) 
	          return;

          var newTarget = ActualTarget;

          if (ReferenceEquals(oldTarget, newTarget) == false)
            OnActualTargetChanged(oldTarget, newTarget);
        }
        finally
        {
          OnApiPropertyChanged(nameof(Target));
        }
      }
    }

    [ApiProperty]
    public string TargetName
    {
      get => SubjectResolver.GetSubjectName(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldTarget = isAppliedOrQueried ? ActualTarget : null;

          SubjectResolver.SetSubjectName(this, value);

          if (isAppliedOrQueried == false) 
	          return;

          var newTarget = ActualTarget;

          if (ReferenceEquals(oldTarget, newTarget) == false)
            OnActualTargetChanged(oldTarget, newTarget);
        }
        finally
        {
          OnApiPropertyChanged(nameof(TargetName));
        }
      }
    }

    [ApiProperty]
    public Transition Transition
    {
      get
      {
        if (IsTransitionSet == false)
          return null;

        if (_runtimeTransitionStore is Transition transition)
          return transition;

        var runtimeSetter = _runtimeTransitionStore as RuntimeSetter;
        return runtimeSetter?.Transition;
      }
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldTransition = isAppliedOrQueried ? ActualTransition : null;

          if (_runtimeTransitionStore is RuntimeSetter runtimeSetter)
            runtimeSetter.Transition = value;
          else
            _runtimeTransitionStore = value;

          IsTransitionSet = true;

          if (isAppliedOrQueried == false) 
	          return;

          var newTransition = ActualTransition;

          if (ReferenceEquals(oldTransition, newTransition) == false)
            OnActualTransitionChanged(oldTransition, newTransition);
        }
        finally
        {
          OnApiPropertyChanged(nameof(Transition));
        }
      }
    }

    [ApiProperty]
    public object Value
    {
      get => ValueResolver.GetValue(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldValue = isAppliedOrQueried ? ActualValue : null;

          ValueResolver.SetValue(this, value);

          if (isAppliedOrQueried == false)
	          return;

          var newValue = ActualValue;

          if (ReferenceEquals(oldValue, newValue) == false)
            OnActualValueChanged(oldValue, newValue);
        }
        finally
        {
          OnApiPropertyChanged(nameof(Value));
        }
      }
    }

    private ValueKind ValueKind
    {
      get => PackedDefinition.ValueKind.GetValue(PackedValue);
      set => PackedDefinition.ValueKind.SetValue(ref PackedValue, value);
    }

    [ApiProperty]
    public string ValuePath
    {
      get => ValueResolver.GetValuePath(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldValuePath = isAppliedOrQueried ? ActualValuePath : null;

          ValueResolver.SetValuePath(this, value);

          if (isAppliedOrQueried == false) 
	          return;

          var newValuePath = ActualValuePath;

          if (string.Equals(oldValuePath, newValuePath) == false)
            OnActualValuePathChanged(oldValuePath, newValuePath);
        }
        finally
        {
          OnApiPropertyChanged(ValuePath);
        }
      }
    }

    [ApiProperty]
    public ValuePathSource ValuePathSource
    {
      get => ValueResolver.GetValuePathSource(this);
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;
					var oldValuePathSource = isAppliedOrQueried ? ActualValuePathSource : ValuePathSource.ThemeResource;

          ValueResolver.SetValuePathSource(this, value);

          if (isAppliedOrQueried == false) 
	          return;

          var newValuePathSource = ActualValuePathSource;

          if (oldValuePathSource != newValuePathSource)
            OnActualValuePathSourceChanged(oldValuePathSource, newValuePathSource);
        }
        finally
        {
          OnApiPropertyChanged(nameof(ValuePathSource));
        }
      }
    }

    [ApiProperty]
    public string VisualState
    {
      get
      {
        var visualStateIndex = VisualStateIndex;

        return visualStateIndex > 0 ? VisualStateMap.GetStateName(visualStateIndex) : null;
      }
      set
      {
        try
        {
          var isAppliedOrQueried = IsAppliedOrQueried;

          var oldVisualState = isAppliedOrQueried ? ActualVisualState : null;

          VisualStateIndex = VisualStateMap.GetStateIndex(value);

          if (isAppliedOrQueried == false) 
	          return;

          var newVisualState = ActualVisualState;

          if (string.Equals(oldVisualState, newVisualState) == false)
            OnActualVisualStateChanged(oldVisualState, newVisualState);
        }
        finally
        {
          OnApiPropertyChanged(nameof(VisualState));
        }
      }
    }

    private uint VisualStateIndex
    {
      get => PackedDefinition.VisualStateIndex.GetValue(PackedValue);
      set => PackedDefinition.VisualStateIndex.SetValue(ref PackedValue, value);
    }

    #endregion

    #region  Methods

    private void BuildActualValuePath(List<string> valuePath)
    {
      var current = this;

      valuePath.Clear();

      do
      {
        if (ValueResolver.IsSpecified(current))
        {
          var currentValuePath = ValueResolver.GetValuePath(current);

          if (currentValuePath.IsNullOrEmpty() == false)
            valuePath.Add(currentValuePath);
        }

        current = current.GetActualParent();
      } while (current != null);

      valuePath.Reverse();
    }

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var setterSource = (PropertyValueSetter) source;

      SubjectResolver.CopyFrom(this, setterSource);
      PropertyResolver.CopyFrom(this, setterSource);
      ValueResolver.CopyFrom(this, setterSource);

      if (setterSource.IsOverriden)
        IsOverriden = true;
      if (setterSource.IsPrioritySet)
        Priority = setterSource.Priority;
      if (setterSource.IsVisualStateSet)
        VisualStateIndex = setterSource.VisualStateIndex;
      if (setterSource.IsTransitionSet)
        Transition = setterSource.Transition;
    }

    protected PropertyValueSetter FindSpecified(Func<PropertyValueSetter, bool> predicate)
    {
      var current = this;

      do
      {
        if (predicate(current))
          break;

        current = current.GetActualParent();
      } while (current != null);

      return current ?? this;
    }

    protected virtual void FlattenProperties(PropertyValueSetter setterSource)
    {
      {
        var subjectSpecified = setterSource.FindSpecified(SubjectResolver.IsSpecified);

        if (SubjectResolver.IsSpecified(subjectSpecified))
          SubjectResolver.CopyFrom(this, subjectSpecified);
      }

      {
        var propertySpecified = setterSource.FindSpecified(PropertyResolver.IsSpecified);

        if (PropertyResolver.IsSpecified(propertySpecified))
          PropertyResolver.CopyFrom(this, propertySpecified);
      }

      var actualValuePath = setterSource.ActualValuePath;

      if (actualValuePath.IsNullOrEmpty() == false)
      {
        ValuePath = actualValuePath;
        ValuePathSource = setterSource.ActualValuePathSource;
      }
      else
      {
        var valueSpecified = setterSource.FindSpecified(ValueResolver.IsSpecified);

        if (ValueResolver.IsSpecified(valueSpecified))
          ValueResolver.CopyFrom(this, valueSpecified);
      }

      {
        var prioritySpecified = setterSource.FindSpecified(GetPrioritySpecified);

        if (GetPrioritySpecified(prioritySpecified))
          Priority = prioritySpecified.Priority;
      }

      {
        var visualStateSpecified = setterSource.FindSpecified(GetVisualStateSpecified);

        if (GetVisualStateSpecified(visualStateSpecified))
          VisualState = visualStateSpecified.VisualState;
      }

      {
        var transitionSpecified = setterSource.FindSpecified(GetTransitionSpecified);

        if (GetTransitionSpecified(transitionSpecified))
          Transition = transitionSpecified.Transition;
      }
    }

    private PropertyValueSetter GetActualParent()
    {
      return Parent as SetterGroup;
    }

    private static bool GetPrioritySpecified(PropertyValueSetter s)
    {
      return s.IsPrioritySet;
    }

    private static bool GetTransitionSpecified(PropertyValueSetter s)
    {
      return s.IsTransitionSet;
    }

    private static bool GetVisualStateSpecified(PropertyValueSetter s)
    {
      return s.IsVisualStateSet;
    }

    protected override void InitFromServiceProvider(IServiceProvider serviceProvider)
    {
      base.InitFromServiceProvider(serviceProvider);

      if (!(Property is string stringProperty))
        return;

      var property = DependencyPropertyUtils.ResolveAttachedDependencyProperty(stringProperty, serviceProvider);

      if (property != null)
        Property = property;
    }

    protected virtual void OnActualPriorityChanged(short oldPriority, short newPriority)
    {
    }

    protected virtual void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
    {
      Reapply();
    }

    protected virtual void OnActualTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
    {
      PropertyResolver.UnResolveProperty(this);
      Reapply();
    }

    protected virtual void OnActualTransitionChanged(Transition oldTransition, Transition newTransition)
    {
    }

    protected virtual void OnActualValueChanged(object oldValue, object newValue)
    {
    }

    protected virtual void OnActualValuePathChanged(string oldValuePath, string newValuePath)
    {
    }

    protected virtual void OnActualValuePathSourceChanged(ValuePathSource oldValuePathSource,
      ValuePathSource newValuePathSource)
    {
    }

    protected virtual void OnActualVisualStateChanged(string oldVisualState, string newVisualState)
    {
    }

    internal void OnParentActualPriorityChanged(short oldPriority, short newPriority)
    {
      if (IsPrioritySet == false)
        OnActualPriorityChanged(oldPriority, newPriority);
    }

    internal void OnParentActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
    {
      if (PropertyResolver.IsSpecified(this) == false)
        OnActualPropertyChanged(oldProperty, newProperty);
    }

    internal void OnParentActualTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
    {
      if (SubjectResolver.IsSpecified(this) == false)
        OnActualTargetChanged(oldTarget, newTarget);
    }

    internal void OnParentActualTransitionChanged(Transition oldTransition, Transition newTransition)
    {
      if (IsTransitionSet == false)
        OnActualTransitionChanged(oldTransition, newTransition);
    }

    internal void OnParentActualValueChanged(object oldValue, object newValue)
    {
      if (ValueResolver.IsSpecified(this) == false)
        OnActualValueChanged(oldValue, newValue);
    }

    internal void OnParentActualValuePathChanged(string oldValuePath, string newValuePath)
    {
      if (ValueResolver.IsSpecified(this) == false)
        OnActualValuePathChanged(oldValuePath, newValuePath);
    }

    internal void OnParentActualValuePathSourceChanged(ValuePathSource oldValuePathSource,
      ValuePathSource newValuePathSource)
    {
      if (ValueResolver.IsSpecified(this) == false)
        OnActualValuePathSourceChanged(oldValuePathSource, newValuePathSource);
    }

    internal void OnParentActualVisualStateChanged(string oldVisualState, string newVisualState)
    {
      if (IsVisualStateSet == false)
        OnActualVisualStateChanged(oldVisualState, newVisualState);
    }

    protected void Reapply()
    {
      if (IsApplied)
      {
        Undo();
        Apply();
      }
      else if (IsApplyQueried)
      {
        Apply();
      }
    }

    internal override DependencyProperty ResolveProperty(Type targetType)
    {
      return PropertyResolver.ResolveProperty(FindSpecified(PropertyResolver.IsSpecified), targetType);
    }

    public override string ToString()
    {
      return DebugView;
    }

    internal override void UnloadCore(IInteractivityRoot root)
    {
      base.UnloadCore(root);

      PropertyResolver.UnResolveProperty(this);
      SubjectResolver.UnResolveSubject(this);
    }

    #endregion

    #region Interface Implementations

    #region IInteractivitySubject

    // ReSharper disable once ConvertToAutoProperty
    object IInteractivitySubject.SubjectStore
    {
      get => MutableData;
      set => MutableData = value;
    }

    SubjectKind IInteractivitySubject.SubjectKind
    {
      get => SubjectKind;
      set => SubjectKind = value;
    }

    void IInteractivitySubject.OnSubjectChanged(DependencyObject oldTargetSource, DependencyObject newTargetSource)
    {
      if (IsAppliedOrQueried)
        OnActualTargetChanged(oldTargetSource, newTargetSource);
    }

    #endregion

    #region IPropertySubject

    DependencyObject IPropertySubject.ActualSubject => ActualTarget;

    PropertyKind IPropertySubject.PropertyKind
    {
      get => PropertyKind;
      set => PropertyKind = value;
    }

    // ReSharper disable once ConvertToAutoProperty
    object IPropertySubject.PropertyStore
    {
      get => _propertyStore;
      set => _propertyStore = value;
    }

    void IPropertySubject.OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
    {
      if (IsAppliedOrQueried)
        OnActualPropertyChanged(oldProperty, newProperty);
    }

    #endregion

    #region IProvideValueTarget

    object IProvideValueTarget.TargetObject => ActualTarget;

    object IProvideValueTarget.TargetProperty => ActualProperty;

    #endregion

    #region IServiceProvider

    object IServiceProvider.GetService(Type serviceType)
    {
      return serviceType == typeof(IProvideValueTarget) ? this : null;
    }

    #endregion

    #region IValueSetter

    // ReSharper disable once ConvertToAutoProperty
    object IValueSetter.ValueStore
    {
      get => _valueStore;
      set => _valueStore = value;
    }

    ValueKind IValueSetter.ValueKind
    {
      get => ValueKind;
      set => ValueKind = value;
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedPriorityDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedUShortItemDefinition Index;
      public static readonly PackedShortItemDefinition Priority;

      #endregion

      #region Ctors

      static PackedPriorityDefinition()
      {
        var priorityAllocator = new PackedValueAllocator();

        Index = priorityAllocator.AllocateUShortItem();
        Priority = priorityAllocator.AllocateShortItem();
      }

      #endregion
    }

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;
      public static readonly PackedEnumItemDefinition<PropertyKind> PropertyKind;
      public static readonly PackedEnumItemDefinition<ValueKind> ValueKind;
      public static readonly PackedBoolItemDefinition IsPrioritySet;
      public static readonly PackedBoolItemDefinition IsTransitionSet;
      public static readonly PackedUIntItemDefinition VisualStateIndex;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = GetAllocator<PropertyValueSetter>();

        SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
        PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
        ValueKind = allocator.AllocateEnumItem<ValueKind>();
        IsPrioritySet = allocator.AllocateBoolItem();
        IsTransitionSet = allocator.AllocateBoolItem();

#if INTERACTIVITY_DEBUG
        VisualStateIndex = allocator.AllocateUIntItem(512);
#else
        VisualStateIndex = allocator.AllocateUIntItem(1024);
#endif
      }

      #endregion
    }

    #endregion
  }
}