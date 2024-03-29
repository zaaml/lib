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
using TemplateBindingExtension = Zaaml.PresentationCore.Data.MarkupExtensions.TemplateBindingExtension;

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
		private static readonly PropertyInfo SetterValuePropertyInfo = typeof(NativeSetter).GetProperty("Value");

		static Setter()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		private InteractivityObject CloneSource
		{
			get => MutableData?.GetType() == GetType() ? (InteractivityObject)MutableData : null;
			set => MutableData = value;
		}

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
			runtimeSetter.EffectiveValue.KeepAlive = ActualVisualStateTrigger != null;

			runtimeSetter.LeaveTransitionContext(useTransitions);

			return true;
		}

		private void AttachVisualStateObserver(IServiceProvider root = null)
		{
			if (ActualVisualStateTrigger == null)
				return;

			IsVisualStateTriggerEnabled = false;

			var vso = root?.GetService<IVisualStateObserver>() ?? GetService<IVisualStateObserver>();

			vso?.AttachListener(this);

			IsVisualStateObserverAttached = true;
		}

		private uint CalcActualPriority()
		{
			return RuntimeSetter.MergePriorityOrder(ActualPriority, Index);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var setterSource = (Setter)source;
			var cloneSource = setterSource.CloneSource;

			if (cloneSource != null)
				CloneSource = cloneSource;
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

			if (string.IsNullOrEmpty(ActualVisualStateTrigger) == false)
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
						extension = new ThemeResourceExtension { Key = themeResourceKey.Key };
						break;
					case ValuePathSource.Skin:
						extension = new SelfBindingExtension { Path = new PropertyPath(Extension.ActualSkinProperty), Converter = SkinResourceConverter.Instance, ConverterParameter = themeResourceKey };
						break;
					case ValuePathSource.TemplateSkin:
						extension = new TemplateBindingExtension { Path = new PropertyPath(Extension.ActualSkinProperty), Converter = SkinResourceConverter.Instance, ConverterParameter = themeResourceKey };
						break;
					case ValuePathSource.TemplateExpando:
						extension = new TemplateExpandoBindingExtension { Path = themeResourceKey.Key };
						break;
					case ValuePathSource.Expando:
						extension = new SelfExpandoBindingExtension { Path = themeResourceKey.Key };
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

			IsVisualStateTriggerEnabled = true;
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

		private ISetterValueProvider GetRuntimeValueProvider(Context context)
		{
			ISetterValueProvider resolvedValueProvider = null;

			try
			{
				if (IsInStyle)
				{
					var styleSetterSource = (Setter)CloneSource;

					if (styleSetterSource != null)
					{
						resolvedValueProvider = SetterValueResolver.ResolveValueProvider(styleSetterSource);

						if (resolvedValueProvider != null)
							return resolvedValueProvider;
					}
				}

				resolvedValueProvider = SetterValueResolver.ResolveValueProvider(this);

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
			return ActualValuePathSource switch
			{
				ValuePathSource.ThemeResource => ThemeManager.GetThemeResourceReference(context.ValuePath),
				ValuePathSource.Skin => null,
				ValuePathSource.TemplateSkin => null,
				ValuePathSource.TemplateExpando => new ExpandoValueProvider(InteractivityTarget.GetTemplatedParent(), context.ValuePath),
				ValuePathSource.Expando => new ExpandoValueProvider(ActualTarget, context.ValuePath),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			UpdateClassTrigger();
			AttachVisualStateObserver(root);
		}

		protected override void OnActualClassTriggerChanged(string oldClassTrigger, string newClassTrigger)
		{
			base.OnActualClassTriggerChanged(oldClassTrigger, newClassTrigger);

			UpdateClassTrigger();
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

		protected override void OnActualVisualStateTriggerChanged(string oldVisualStateTrigger, string newVisualStateTrigger)
		{
			base.OnActualVisualStateTriggerChanged(oldVisualStateTrigger, newVisualStateTrigger);

			DetachVisualStateObserver();
			AttachVisualStateObserver();
		}

		internal void OnClassChangedInternal()
		{
			UpdateClassTrigger();
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

			UpdateClassTrigger();
			DetachVisualStateObserver();

			base.UnloadCore(root);
		}

		private void UpdateClassTrigger()
		{
			var actualClassTrigger = ActualClassTrigger;
			var interactivityTarget = InteractivityTarget;

			IsClassTriggerEnabled = interactivityTarget == null ||
			                        actualClassTrigger == null ||
			                        Extension.GetActualClass(interactivityTarget)?.HasClass(actualClassTrigger) == true;
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

			if (actualValuePathSource is ValuePathSource.Skin or ValuePathSource.TemplateSkin)
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

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var actualPropertyType = ActualPropertyType;

			return actualPropertyType == null ? value : value.XamlConvert(actualPropertyType);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		string IVisualStateListener.VisualStateName => ActualVisualStateTrigger;

		void IVisualStateListener.EnterState(bool useTransitions)
		{
			UseTransitions = useTransitions;
			IsVisualStateTriggerEnabled = true;
			UseTransitions = false;
		}

		void IVisualStateListener.LeaveState(bool useTransitions)
		{
			UseTransitions = useTransitions;
			IsVisualStateTriggerEnabled = false;
			UseTransitions = false;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsVisualStateObserverAttached;
			public static readonly PackedBoolItemDefinition UseTransitions;

			static PackedDefinition()
			{
				var allocator = GetAllocator<Setter>();

				IsVisualStateObserverAttached = allocator.AllocateBoolItem();
				UseTransitions = allocator.AllocateBoolItem();
			}
		}
	}
}