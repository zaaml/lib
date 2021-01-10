// <copyright file="RuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.PresentationCore.Animation;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.PresentationCore.Interactivity
{
	internal abstract class RuntimeSetter : IDisposable
	{
		private static readonly List<RuntimeSetter> UseTransitionsList = new List<RuntimeSetter>();

		private EffectiveValue _effectiveValue;
		private RuntimeSetter _nextSetter;
		private uint _priority;
		private Transition _transition;
		private object _valueStore = Unset.Value;

		public abstract long ActualPriority { get; }

		private RuntimeSetterTransition ActualRuntimeTransition => ValueStore as RuntimeSetterTransition;

		public object ActualValueStore
		{
			get
			{
				if (ValueStore is RuntimeSetterException exception)
					return exception.ValueStore.GetSetValueOrDefault();

				if (ValueStore is RuntimeSetterTransition transition)
					return transition.ValueStore.GetSetValueOrDefault();

				return ValueStore.GetSetValueOrDefault();
			}
		}

		public EffectiveValue EffectiveValue
		{
			get => _effectiveValue;
			private set
			{
				if (ReferenceEquals(_effectiveValue, value))
					return;

				_effectiveValue?.RemoveSetter(this);

				_effectiveValue = value;

				_effectiveValue?.AddSetter(this);
			}
		}

		public abstract bool IsWeak { get; }

		// ReSharper disable once ConvertToAutoProperty
		public RuntimeSetter NextSetter
		{
			get => _nextSetter;
			set => _nextSetter = value;
		}

		public uint Priority
		{
			get => _priority;
			set
			{
				if (_priority == value)
					return;

				_priority = value;

				_effectiveValue?.OnRuntimeSetterPriorityChanged(this);
			}
		}

		public Type TargetPropertyType => EffectiveValue != null ? EffectiveValue.Property.GetPropertyType() : typeof(object);

		// ReSharper disable once ConvertToAutoProperty
		public Transition Transition
		{
			get => _transition;
			set => _transition = value;
		}

		public object Value
		{
			get
			{
				var valueStore = ActualValueStore;

				return valueStore is IServiceProvider ? null : valueStore;
			}
			set
			{
				if (AssignValueOrProvider(value))
					UpdateEffectiveValue();
			}
		}

		internal Exception ValueException => (ValueStore as RuntimeSetterException)?.Exception;

		public ISetterValueProvider ValueProvider
		{
			get => ActualValueStore as ISetterValueProvider;
			set
			{
				if (AssignValueOrProvider(value))
					UpdateEffectiveValue();
			}
		}

		// ReSharper disable once ConvertToAutoProperty
		public object ValueStore
		{
			get => _valueStore;
			private set => _valueStore = value;
		}

		public void Apply(EffectiveValue effectiveValue)
		{
			EffectiveValue = effectiveValue;
		}

		private bool AssignValueOrProvider(object value)
		{
			if (ReferenceEquals(ValueStore, value))
				return false;

			DetachValueProvider();

			UpdateValueStore(value);

			AttachValueProvider();

			return true;
		}

		public void AssignValueOrProvider(Setter.Context setter, bool force)
		{
			if (ValueStore.IsSet() && force == false)
				return;

			var valueProvider = setter.RuntimeValueProvider;

			if (valueProvider != null)
			{
				if (AssignValueOrProvider(valueProvider))
					UpdateEffectiveValue();

				return;
			}

			if (AssignValueOrProvider(setter.RuntimeValue))
				UpdateEffectiveValue();
		}

		private void AttachValueProvider()
		{
			var valueProvider = ValueProvider;

			if (valueProvider != null && valueProvider.IsDynamic())
				valueProvider.Attach(this);
		}

		private void DetachValueProvider()
		{
			var valueProvider = ValueProvider;

			if (valueProvider != null && valueProvider.IsDynamic())
				valueProvider.Detach(this);
		}

		public void EnterTransitionContext(bool useTransitions)
		{
			if (useTransitions)
				UseTransitionsList.Add(this);
		}

		public void LeaveTransitionContext(bool useTransitions)
		{
			if (useTransitions)
				UseTransitionsList.Remove(this);
		}

		public static uint MergePriorityOrder(short priority, ushort order)
		{
			unchecked
			{
				int intPriority = priority;
				intPriority += short.MaxValue + 1;

				return ((uint) intPriority << 16) | order;
			}
		}

		public void OnAttached()
		{
		}

		public void OnDetached()
		{
			ActualRuntimeTransition?.Stop();
		}

		public void OnProviderValueChanged()
		{
			UpdateEffectiveValue();
		}

		public void OnTransitionCompleted()
		{
			UpdateEffectiveValue();

			var transition = (RuntimeSetterTransition) ValueStore;

			ValueStore = transition.ValueStore;
		}

		public object ProvideValue(object currentValue)
		{
			if (ValueStore is RuntimeSetterException)
				return ValueStore;

			var runtimeTransition = ActualRuntimeTransition;

			if (runtimeTransition != null)
				return runtimeTransition.CurrentValue;

			try
			{
				var valueProvider = ValueProvider;

				if (valueProvider != null)
				{
					var provideValue = ValueProvider.ProvideValue(this);

					if (provideValue is Exception exception)
						ValueStore = new RuntimeSetterException(ValueStore, exception);
					else
						return RunTransition(currentValue, provideValue);
				}

				if (Value is Binding || Value is BindingExpression)
					ValueStore = Value;
				else
				{
					var convertResult = XamlStaticConverter.TryConvertValue(Value, TargetPropertyType);

					ValueStore = convertResult.IsValid ? convertResult.Result : new RuntimeSetterException(ValueStore, convertResult.Exception);
				}

				if (ValueStore is RuntimeSetterException)
					return ValueStore;

				return RunTransition(currentValue, ValueStore);
			}
			catch (Exception e)
			{
				ValueStore = new RuntimeSetterException(ValueStore, e);
			}

			return ValueStore;
		}

		public static uint RemoveOrder(uint mergedPriority)
		{
			return mergedPriority & 0xFFFF0000;
		}

		public void ResetValueProvider()
		{
			if (ValueProvider != null)
				ValueProvider = null;
		}

		private object RunTransition(object currentValue, object value)
		{
			if (Transition == null || Equals(currentValue, value))
				return value;

			if (Transition.BeginTime == TimeSpan.Zero && Transition.Duration.HasTimeSpan && Transition.Duration.TimeSpan == TimeSpan.Zero)
				return value;
			
			if (currentValue != null && value != null && currentValue.GetType() != value.GetType())
			{
				if (currentValue is Binding == false && value is Binding == false && currentValue is BindingExpression == false && value is BindingExpression == false)
					return value;
			}

			if (UseTransitions(_effectiveValue.Target) == false)
				return value;

			var runtimeTransition = RuntimeSetterTransition.RunTransition(this, currentValue, value);

			if (runtimeTransition == null)
				return value;

			ValueStore = runtimeTransition;

			return runtimeTransition.CurrentValue;
		}

		public bool SetAnimatedValue(object animatorCurrent)
		{
			return UpdateEffectiveValue();
		}

		public void Undo()
		{
			ActualRuntimeTransition?.Stop();
			EffectiveValue = null;
		}

		private bool UpdateEffectiveValue()
		{
			if (_effectiveValue?.IsCurrentSetterValue(this) != true)
				return false;

			_effectiveValue.UpdateEffectiveValue();

			return true;
		}

		public void UpdateValue(object value)
		{
			if (ValueProvider != null || ActualRuntimeTransition != null)
				return;

			ValueStore = value;
		}

		private void UpdateValueStore(object value)
		{
			ActualRuntimeTransition?.Stop();
			ValueStore = value ?? Unset.Value;
		}

		private bool UseTransitions(DependencyObject target)
		{
			if (UseTransitionsList.Contains(this))
				return true;

			var freTarget = target as FrameworkElement;

			if (freTarget == null)
				return true;

			var templatedParent = target.GetTemplatedParent();

			if (templatedParent != null)
				return templatedParent.GetInteractivityService().UseTransitions;

			var interactivityService = freTarget.GetInteractivityService();
			var elementRoot = interactivityService.ElementRoot;

			return elementRoot?.XamlElementRoot?.GetInteractivityService().UseTransitions ?? interactivityService.UseTransitions;
		}

		public void Dispose()
		{
			Undo();

			ResetValueProvider();

#if SILVERLIGHT
      _valueStore.ResetInheritanceParent();
#endif

			UpdateValueStore(null);
		}

		private class RuntimeSetterException : Exception
		{
			public RuntimeSetterException(object valueStore, Exception exception) : base(exception.Message)
			{
				ValueStore = valueStore;
				Exception = exception;
			}

			public Exception Exception { get; }

			public object ValueStore { get; }
		}
	}
}