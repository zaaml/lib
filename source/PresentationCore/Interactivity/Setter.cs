// <copyright file="Setter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Binding = System.Windows.Data.Binding;
using NativeSetter = System.Windows.Setter;

// TODO Priority calculation: style trigger setter should have higher priority then style setter

// TODO Implement Switch/Case setter for simple triggering value, example below. Case property should be mixed in a single field store with VisualState.

//<zm:GroupSetter Switch="{zm:TemplateExpando Path=State" >
//  <zm:Setter Case="Normal" Value="Red" />
//	<zm:Setter Case="Selected" Value="Green" />
//	<zm:Setter Case="Focused" Value="Blue" />
//</zm:GroupSetter>

namespace Zaaml.PresentationCore.Interactivity
{
  public sealed partial class Setter : PropertyValueSetter, IValueConverter, IVisualStateListener
  {
    #region Static Fields and Constants

    private static readonly PropertyInfo SetterValuePropertyInfo = typeof(NativeSetter).GetProperty("Value");

    #endregion

    #region Ctors

    static Setter()
    {
      RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
    }

    #endregion

    #region Properties

    private bool IsVisualStateObserverAttached
    {
      get => PackedDefinition.IsVisualStateObserverAttached.GetValue(PackedValue);
      set => PackedDefinition.IsVisualStateObserverAttached.SetValue(ref PackedValue, value);
    }

    private RuntimeSetter RuntimeSetter => RuntimeTransitionStore as RuntimeSetter;

    private bool UseTransitions
    {
      get => PackedDefinition.UseTransitions.GetValue(PackedValue);
      set => PackedDefinition.UseTransitions.SetValue(ref PackedValue, value);
    }

    #endregion

    #region  Methods

    protected override bool ApplyCore()
    {
#if INTERACTIVITY_DEBUG
			if (Debug)
			{
			}
#endif
			using var context = Context.Get(this);

	    var actualTarget = context.Target;

	    if (actualTarget == null)
		    return false;

	    var actualProperty = context.Property;

	    if (actualProperty == null)
		    return false;

	    var runtimeSetter = RuntimeSetter;

	    if (runtimeSetter == null)
	    {
		    runtimeSetter = CreateRuntimeSetter();
		    runtimeSetter.Priority = CalcActualPriority();

		    runtimeSetter.Transition = Transition;
		    RuntimeTransitionStore = runtimeSetter;
	    }

	    var useTransitions = UseTransitions;

	    runtimeSetter.EnterTransitionContext(useTransitions);

	    runtimeSetter.Transition = ActualTransition;
	    runtimeSetter.AssignValueOrProvider(context, false);

	    runtimeSetter.Apply(EffectiveValue.GetEffectiveValue(actualTarget, actualProperty));
	    runtimeSetter.EffectiveValue.KeepAlive = ActualVisualState != null;

	    runtimeSetter.LeaveTransitionContext(useTransitions);

	    return true;
    }

    private void AttachVisualStateObserver(IServiceProvider root = null)
    {
      if (ActualVisualState == null) 
	      return;

      IsEnabled = false;

      var vso = root?.GetService<IVisualStateObserver>() ?? GetService<IVisualStateObserver>();
      vso?.AttachListener(this);

      IsVisualStateObserverAttached = true;
    }

    private uint CalcActualPriority()
    {
      return RuntimeSetter.MergePriorityOrder(ActualPriority, Index);
    }

    protected override InteractivityObject CreateInstance()
    {
      return new Setter();
    }

    internal NativeSetter CreateNativeStyleSetter(Type targetType)
    {
#if INTERACTIVITY_DEBUG
      if (Debug)
        return null;
#endif

      if (string.IsNullOrEmpty(ActualVisualState) == false)
        return null;

      var dependencyProperty = ResolveProperty(targetType);

      if (dependencyProperty == null)
      {
        LogService.LogWarning($"Unable resolve property for setter: {this}");

        return null;
      }

      var themeResourceKey = ActualThemeResourceKey;

      if (themeResourceKey.IsEmpty == false)
      {
        var nativeSetter = new NativeSetter
        {
          Property = dependencyProperty
        };

        BindingMarkupExtension extension = null;

        switch (ActualValuePathSource)
        {
          case ValuePathSource.ThemeResource:
            extension = new ThemeResourceExtension {Key = themeResourceKey.Key};
            break;
          case ValuePathSource.Skin:
            extension = new SelfBindingExtension {Path = new PropertyPath(Extension.ActualSkinProperty), Converter = SkinResourceConverter.Instance, ConverterParameter = themeResourceKey};
            break;
          case ValuePathSource.TemplateSkin:
	          extension = new Data.MarkupExtensions.TemplateBindingExtension {Path = new PropertyPath(Extension.ActualSkinProperty), Converter = SkinResourceConverter.Instance, ConverterParameter = themeResourceKey};
	          break;
          case ValuePathSource.TemplateExpando:
            extension = new TemplateExpandoBindingExtension {Path = themeResourceKey.Key};
            break;
          case ValuePathSource.Expando:
            extension = new SelfExpandoBindingExtension {Path = themeResourceKey.Key};
            break;
        }

        if (extension != null)
          nativeSetter.Value = extension.GetBinding(nativeSetter, SetterValuePropertyInfo);

        return nativeSetter;
      }

      var value = Value;

      if (value is MarkupExtension markupExtension)
      {
        var nativeSetter = new NativeSetter
        {
          Property = dependencyProperty
        };

        if (markupExtension is BindingMarkupExtension bindingMarkupExtension)
          nativeSetter.Value = bindingMarkupExtension.GetBinding(nativeSetter, SetterValuePropertyInfo);
        else if (markupExtension is BindingBase)
          nativeSetter.Value = markupExtension;
        else
          using (var serviceProvider = TargetServiceProvider.GetServiceProvider(nativeSetter, SetterValuePropertyInfo))
            nativeSetter.Value = markupExtension.ProvideValue(serviceProvider);

        return nativeSetter;
      }

      var convertedValue = XamlStaticConverter.TryConvertValue(value, dependencyProperty.GetPropertyType());

      if (convertedValue.IsFailed)
      {
        LogService.LogWarning($"Unable convert value for setter: {this}");

        return null;
      }

      return new NativeSetter
      {
        Property = dependencyProperty,
        Value = convertedValue.Result
      };
    }

    private RuntimeSetter CreateRuntimeSetter()
    {
      IRuntimeSetterFactory factory = null;

      var current = Parent;

      while (current != null)
      {
        factory = current as IRuntimeSetterFactory;

        if (factory != null)
          break;

        current = current.Parent;
      }

      return factory?.CreateSetter() ?? new DefaultRuntimeSetter();
    }

    private void DetachVisualStateObserver()
    {
      if (IsVisualStateObserverAttached)
        GetService<IVisualStateObserver>()?.DetachListener(this);

      IsEnabled = true;
      IsVisualStateObserverAttached = false;
    }

    internal Setter Flatten()
    {
      var copy = DeepClone<Setter>();

      copy.FlattenProperties(this);

      copy.CloneSource = copy;

      return copy;
    }

    private object GetRuntimeValue(Context context)
    {
      var value = context.Value;

      if (string.IsNullOrEmpty(context.ValuePath) == false && (context.ValuePathSource == ValuePathSource.Skin || context.ValuePathSource == ValuePathSource.TemplateSkin))
        return GetSkinValue(context.ValuePathSource);

      if (value is BindingMarkupExtension bindingMarkupExtension)
        value = bindingMarkupExtension.GetBinding(context.Target, context.Property);

      var binding = value as Binding;

      if (binding == null)
        return value;

      if (binding.Mode == BindingMode.TwoWay)
        throw new NotSupportedException();

      if (binding.Source == null && binding.RelativeSource == null && binding.ElementName == null)
      {
        binding = binding.CloneBinding();
        binding.RelativeSource = XamlConstants.Self;
      }

      return binding;
    }

    protected internal override void CopyMembersOverride(InteractivityObject source)
    {
      base.CopyMembersOverride(source);

      var setterSource = (Setter) source;
      var cloneSource = setterSource.CloneSource;

      if (cloneSource != null)
        CloneSource = cloneSource;
    }

    private InteractivityObject CloneSource
    {
      get => MutableData?.GetType() == GetType() ? (InteractivityObject)MutableData : null;
      set => MutableData = value;
    }

    private ISetterValueProvider GetRuntimeValueProvider(Context context)
    {
      ISetterValueProvider resolvedValueProvider = null;

      try
      {
        if (IsInStyle)
        {
          var styleSetterSource = (Setter) CloneSource;

          if (styleSetterSource != null)
          {
            resolvedValueProvider = ValueResolver.ResolveValueProvider(styleSetterSource);

            if (resolvedValueProvider != null)
              return resolvedValueProvider;
          }
        }

        resolvedValueProvider = ValueResolver.ResolveValueProvider(this);

        if (resolvedValueProvider != null)
          return resolvedValueProvider;

        var actualValuePath = context.ValuePath;

        resolvedValueProvider = string.IsNullOrEmpty(actualValuePath) == false ? GetValuePathProvider(context) : context.Value as ISetterValueProvider;

        return resolvedValueProvider;
      }
      finally
      {
        if (resolvedValueProvider is ThemeResourceReference || resolvedValueProvider is ThemeResourceExtension)
          context.InteractivityRoot.InteractivityService.EnsureThemeListener();

        if (context.ValuePathSource == ValuePathSource.Skin || context.ValuePathSource == ValuePathSource.TemplateSkin)
        {
          var elementRoot = context.InteractivityRoot as ElementRoot;

          elementRoot?.EnsureSkinListener();
        }
      }
    }

    private SkinBase GetSkin(ValuePathSource valuePathSource)
    {
      var interactivityRoot = Root;
      var frameworkElementRoot = interactivityRoot as ElementRoot;

      if (valuePathSource == ValuePathSource.TemplateSkin)
	      return Extension.GetActualSkin(frameworkElementRoot?.TemplatedParent);

			if (valuePathSource == ValuePathSource.Skin)
				return Extension.GetActualSkin(interactivityRoot.InteractivityTarget);

			return null;
    }

    private object GetSkinValue(ValuePathSource valuePathSource)
    {
      var skin = GetSkin(valuePathSource);

      return skin?.GetValueInternal(ActualThemeResourceKey);
    }

    private ISetterValueProvider GetValuePathProvider(Context context)
    {
      var actualValuePathSource = ActualValuePathSource;

      switch (actualValuePathSource)
      {
        case ValuePathSource.ThemeResource:
          return ThemeManager.GetThemeReference(context.ValuePath);
        case ValuePathSource.Skin:
          return null;
        case ValuePathSource.TemplateSkin:
	        return null;
        case ValuePathSource.TemplateExpando:
          return new ExpandoValueProvider(InteractivityTarget.GetTemplatedParent(), context.ValuePath);
        case ValuePathSource.Expando:
          return new ExpandoValueProvider(ActualTarget, context.ValuePath);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    internal override void LoadCore(IInteractivityRoot root)
    {
      base.LoadCore(root);

      AttachVisualStateObserver(root);
    }

    protected override void OnActualPriorityChanged(short oldPriority, short newPriority)
    {
      base.OnActualPriorityChanged(oldPriority, newPriority);

      if (RuntimeSetter != null)
        RuntimeSetter.Priority = CalcActualPriority();
    }

    protected override void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
    {
      RuntimeSetter?.ResetValueProvider();

      base.OnActualPropertyChanged(oldProperty, newProperty);
    }

    protected override void OnActualTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
    {
      RuntimeSetter?.ResetValueProvider();

      base.OnActualTargetChanged(oldTarget, newTarget);
    }

    protected override void OnActualValueChanged(object oldValue, object newValue)
    {
      UpdateEffectiveValue();

      base.OnActualValueChanged(oldValue, newValue);
    }

    protected override void OnActualValuePathChanged(string oldValuePath, string newValuePath)
    {
      UpdateEffectiveValue();

      base.OnActualValuePathChanged(oldValuePath, newValuePath);
    }

    protected override void OnActualValuePathSourceChanged(ValuePathSource oldValuePathSource, ValuePathSource newValuePathSource)
    {
      UpdateEffectiveValue();

      base.OnActualValuePathSourceChanged(oldValuePathSource, newValuePathSource);
    }

    protected override void OnActualVisualStateChanged(string oldVisualState, string newVisualState)
    {
      base.OnActualVisualStateChanged(oldVisualState, newVisualState);

      DetachVisualStateObserver();
      AttachVisualStateObserver();
    }

    internal Setter Optimize()
    {
      var expandoProperty = ExpandoProperty;

      if (string.IsNullOrEmpty(expandoProperty) == false)
        Property = DependencyPropertyManager.GetExpandoProperty(expandoProperty);

      return this;
    }

    protected override void UndoCore()
    {
      if (RuntimeSetter == null)
        return;

      RuntimeSetter.Undo();

      var valueProvider = RuntimeSetter.ValueProvider;

      if (valueProvider != null && valueProvider.IsShared() == false && valueProvider.IsLongLife() == false)
        RuntimeSetter.ValueProvider = null;
    }

    internal override void UnloadCore(IInteractivityRoot root)
    {
      var runtimeSetter = RuntimeSetter;

      if (runtimeSetter != null)
      {
        runtimeSetter.Dispose();
        RuntimeTransitionStore = IsTransitionSet ? runtimeSetter.Transition : null;
      }

      DetachVisualStateObserver();

      base.UnloadCore(root);
    }

    private void UpdateEffectiveValue()
    {
	    using var context = Context.Get(this);

	    RuntimeSetter?.AssignValueOrProvider(context, true);
    }

    internal void UpdateSkin(SkinBase newSkin)
    {
      if (RuntimeSetter == null)
        return;

      var actualValuePathSource = ActualValuePathSource;

      if (actualValuePathSource == ValuePathSource.Skin || actualValuePathSource == ValuePathSource.TemplateSkin)
				RuntimeSetter.Value = GetSkinValue(actualValuePathSource);
    }

    internal void UpdateThemeResources()
    {
      if (RuntimeSetter == null)
        return;

      var valueProvider = RuntimeSetter.ValueProvider;

      if (valueProvider is ThemeResourceExtension themeResourceExtension)
        themeResourceExtension.UpdateThemeResource();
      else if (valueProvider is ThemeResourceReference == false)
        return;

      RuntimeSetter.OnProviderValueChanged();
    }

    #endregion

    #region Interface Implementations

    #region IValueConverter

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var actualPropertyType = ActualPropertyType;

      return actualPropertyType == null ? value : value.XamlConvert(actualPropertyType);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion

    #region IVisualStateListener

    string IVisualStateListener.VisualStateName => ActualVisualState;

    void IVisualStateListener.EnterState(bool useTransitions)
    {
      UseTransitions = useTransitions;
      IsEnabled = true;
      UseTransitions = false;
    }

    void IVisualStateListener.LeaveState(bool useTransitions)
    {
      UseTransitions = useTransitions;
      IsEnabled = false;
      UseTransitions = false;
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsVisualStateObserverAttached;
      public static readonly PackedBoolItemDefinition UseTransitions;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = GetAllocator<Setter>();

        IsVisualStateObserverAttached = allocator.AllocateBoolItem();
        UseTransitions = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }
}