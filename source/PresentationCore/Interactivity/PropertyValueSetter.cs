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
		private static readonly uint DefaultPackedValue;

		private uint _priorityStore;
		private object _propertyStore;
		private object _runtimeTransitionStore = Unset.Value;
		private string _trigger;
		private object _valueStore;

		static PropertyValueSetter()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.SubjectKind.SetValue(ref DefaultPackedValue, SubjectKind.Unspecified);
			PackedDefinition.PropertyKind.SetValue(ref DefaultPackedValue, PropertyKind.Unspecified);
			PackedDefinition.ValueKind.SetValue(ref DefaultPackedValue, SetterValueKind.Unspecified);
			PackedDefinition.TriggerKind.SetValue(ref DefaultPackedValue, SetterTriggerKind.Unspecified);
		}

		protected PropertyValueSetter()
		{
			PackedValue |= DefaultPackedValue;
		}

		protected string ActualClassTrigger => FindSpecified(GetClassTriggerSpecified).ClassTrigger;

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

		protected object ActualValue => SetterValueResolver.GetValue(FindSpecified(SetterValueResolver.IsSpecified));

		protected string ActualValuePath => ActualThemeResourceKey.IsEmpty ? null : ActualThemeResourceKey.Key;

		protected ValuePathSource ActualValuePathSource => SetterValueResolver.GetValuePathSource(FindSpecified(SetterValueResolver.IsSpecifiedValuePath));

		protected string ActualVisualStateTrigger => FindSpecified(GetVisualStateTriggerSpecified).VisualStateTrigger;


		[ApiProperty]
		public string ClassTrigger
		{
			get => TriggerKind == SetterTriggerKind.Class ? _trigger : null;
			set
			{
				try
				{
					var isAppliedOrQueried = IsAppliedOrQueried;
					var oldClassTrigger = isAppliedOrQueried ? ActualClassTrigger : null;

					TriggerKind = SetterTriggerKind.Class;
					_trigger = value;

					if (isAppliedOrQueried == false)
						return;

					var newClassTrigger = ActualClassTrigger;

					if (string.Equals(oldClassTrigger, newClassTrigger) == false)
						OnActualClassTriggerChanged(oldClassTrigger, newClassTrigger);
				}
				finally
				{
					OnApiPropertyChanged(nameof(ClassTrigger));
				}
			}
		}

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

				if (VisualStateTrigger != null)
				{
					sb.AppendFormat($"VisualState=\"{VisualStateTrigger}\"");
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

				var oldPriority = isAppliedOrQueried ? ActualPriority : (short)0;

				PackedPriorityDefinition.Index.SetValue(ref _priorityStore, value);

				if (isAppliedOrQueried == false)
					return;

				var newPriority = ActualPriority;

				if (oldPriority != newPriority)
					OnActualPriorityChanged(oldPriority, newPriority);
			}
		}

		protected bool IsClassTriggerSet => TriggerKind == SetterTriggerKind.Class && string.IsNullOrEmpty(_trigger) == false;

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

		protected bool IsVisualStateTriggerSet => TriggerKind == SetterTriggerKind.VisualState && string.IsNullOrEmpty(_trigger) == false;

		internal override DependencyProperty MergeDependencyProperty
		{
			get
			{
				var dependencyProperty = DependencyProperty;

				if (dependencyProperty == null)
					return null;

				var actualVisualState = ActualVisualStateTrigger;

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

					var oldPriority = isAppliedOrQueried ? ActualPriority : (short)0;

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

		private SetterTriggerKind TriggerKind
		{
			get => PackedDefinition.TriggerKind.GetValue(PackedValue);
			set => PackedDefinition.TriggerKind.SetValue(ref PackedValue, value);
		}

		[ApiProperty]
		public object Value
		{
			get => SetterValueResolver.GetValue(this);
			set
			{
				try
				{
					var isAppliedOrQueried = IsAppliedOrQueried;
					var oldValue = isAppliedOrQueried ? ActualValue : null;

					SetterValueResolver.SetValue(this, value);

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

		private SetterValueKind ValueKind
		{
			get => PackedDefinition.ValueKind.GetValue(PackedValue);
			set => PackedDefinition.ValueKind.SetValue(ref PackedValue, value);
		}

		[ApiProperty]
		public string ValuePath
		{
			get => SetterValueResolver.GetValuePath(this);
			set
			{
				try
				{
					var isAppliedOrQueried = IsAppliedOrQueried;
					var oldValuePath = isAppliedOrQueried ? ActualValuePath : null;

					SetterValueResolver.SetValuePath(this, value);

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
			get => SetterValueResolver.GetValuePathSource(this);
			set
			{
				try
				{
					var isAppliedOrQueried = IsAppliedOrQueried;
					var oldValuePathSource = isAppliedOrQueried ? ActualValuePathSource : ValuePathSource.ThemeResource;

					SetterValueResolver.SetValuePathSource(this, value);

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
		public string VisualStateTrigger
		{
			get { return TriggerKind == SetterTriggerKind.VisualState ? _trigger : null; }
			set
			{
				try
				{
					var isAppliedOrQueried = IsAppliedOrQueried;
					var oldVisualStateTrigger = isAppliedOrQueried ? ActualVisualStateTrigger : null;

					TriggerKind = SetterTriggerKind.VisualState;
					_trigger = value;

					if (isAppliedOrQueried == false)
						return;

					var newVisualStateTrigger = ActualVisualStateTrigger;

					if (string.Equals(oldVisualStateTrigger, newVisualStateTrigger) == false)
						OnActualVisualStateTriggerChanged(oldVisualStateTrigger, newVisualStateTrigger);
				}
				finally
				{
					OnApiPropertyChanged(nameof(VisualStateTrigger));
				}
			}
		}

		private void BuildActualValuePath(List<string> valuePath)
		{
			var current = this;

			valuePath.Clear();

			do
			{
				if (SetterValueResolver.IsSpecified(current))
				{
					var currentValuePath = SetterValueResolver.GetValuePath(current);

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

			var setterSource = (PropertyValueSetter)source;

			SubjectResolver.CopyFrom(this, setterSource);
			PropertyResolver.CopyFrom(this, setterSource);
			SetterValueResolver.CopyFrom(this, setterSource);

			if (setterSource.IsOverriden)
				IsOverriden = true;

			if (setterSource.IsPrioritySet)
				Priority = setterSource.Priority;

			if (setterSource.IsVisualStateTriggerSet)
				VisualStateTrigger = setterSource.VisualStateTrigger;

			if (setterSource.IsClassTriggerSet)
				ClassTrigger = setterSource.ClassTrigger;

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
				var valueSpecified = setterSource.FindSpecified(SetterValueResolver.IsSpecified);

				if (SetterValueResolver.IsSpecified(valueSpecified))
					SetterValueResolver.CopyFrom(this, valueSpecified);
			}

			{
				var prioritySpecified = setterSource.FindSpecified(GetPrioritySpecified);

				if (GetPrioritySpecified(prioritySpecified))
					Priority = prioritySpecified.Priority;
			}

			{
				var visualStateSpecified = setterSource.FindSpecified(GetVisualStateTriggerSpecified);

				if (GetVisualStateTriggerSpecified(visualStateSpecified))
					VisualStateTrigger = visualStateSpecified.VisualStateTrigger;
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

		private static bool GetClassTriggerSpecified(PropertyValueSetter s)
		{
			return s.IsClassTriggerSet;
		}

		private static bool GetPrioritySpecified(PropertyValueSetter s)
		{
			return s.IsPrioritySet;
		}

		private static bool GetTransitionSpecified(PropertyValueSetter s)
		{
			return s.IsTransitionSet;
		}

		private static bool GetVisualStateTriggerSpecified(PropertyValueSetter s)
		{
			return s.IsVisualStateTriggerSet;
		}

		protected override void InitFromServiceProvider(IServiceProvider serviceProvider)
		{
			base.InitFromServiceProvider(serviceProvider);

			if (Property is not string stringProperty)
				return;

			var property = DependencyPropertyUtils.ResolveAttachedDependencyProperty(stringProperty, serviceProvider);

			if (property != null)
				Property = property;
		}

		protected virtual void OnActualClassTriggerChanged(string oldClassTrigger, string newClassTrigger)
		{
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

		protected virtual void OnActualVisualStateTriggerChanged(string oldVisualStateTrigger, string newVisualStateTrigger)
		{
		}

		internal void OnParentActualClassTriggerChanged(string oldClassTrigger, string newClassTrigger)
		{
			if (IsClassTriggerSet == false)
				OnActualClassTriggerChanged(oldClassTrigger, newClassTrigger);
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
			if (SetterValueResolver.IsSpecified(this) == false)
				OnActualValueChanged(oldValue, newValue);
		}

		internal void OnParentActualValuePathChanged(string oldValuePath, string newValuePath)
		{
			if (SetterValueResolver.IsSpecified(this) == false)
				OnActualValuePathChanged(oldValuePath, newValuePath);
		}

		internal void OnParentActualValuePathSourceChanged(ValuePathSource oldValuePathSource,
			ValuePathSource newValuePathSource)
		{
			if (SetterValueResolver.IsSpecified(this) == false)
				OnActualValuePathSourceChanged(oldValuePathSource, newValuePathSource);
		}

		internal void OnParentActualVisualStateTriggerChanged(string oldVisualState, string newVisualState)
		{
			if (IsVisualStateTriggerSet == false)
				OnActualVisualStateTriggerChanged(oldVisualState, newVisualState);
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

		private protected override bool TryProvideValue(object target, object targetProperty, IServiceProvider serviceProvider, out object value)
		{
			if (targetProperty is DependencyProperty dependencyProperty)
				Property = dependencyProperty;

			return base.TryProvideValue(target, targetProperty, serviceProvider, out value);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			base.UnloadCore(root);

			PropertyResolver.UnResolveProperty(this);
			SubjectResolver.UnResolveSubject(this);
		}

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

		object IProvideValueTarget.TargetObject => ActualTarget;

		object IProvideValueTarget.TargetProperty => ActualProperty;

		object IServiceProvider.GetService(Type serviceType)
		{
			return serviceType == typeof(IProvideValueTarget) ? this : null;
		}

		// ReSharper disable once ConvertToAutoProperty
		object IValueSetter.ValueStore
		{
			get => _valueStore;
			set => _valueStore = value;
		}

		SetterValueKind IValueSetter.ValueKind
		{
			get => ValueKind;
			set => ValueKind = value;
		}

		private static class PackedPriorityDefinition
		{
			public static readonly PackedUShortItemDefinition Index;
			public static readonly PackedShortItemDefinition Priority;

			static PackedPriorityDefinition()
			{
				var priorityAllocator = new PackedValueAllocator();

				Index = priorityAllocator.AllocateUShortItem();
				Priority = priorityAllocator.AllocateShortItem();
			}
		}

		private static class PackedDefinition
		{
			public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;
			public static readonly PackedEnumItemDefinition<PropertyKind> PropertyKind;
			public static readonly PackedEnumItemDefinition<SetterValueKind> ValueKind;
			public static readonly PackedEnumItemDefinition<SetterTriggerKind> TriggerKind;
			public static readonly PackedBoolItemDefinition IsPrioritySet;
			public static readonly PackedBoolItemDefinition IsTransitionSet;


			static PackedDefinition()
			{
				var allocator = GetAllocator<PropertyValueSetter>();

				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
				ValueKind = allocator.AllocateEnumItem<SetterValueKind>();
				TriggerKind = allocator.AllocateEnumItem<SetterTriggerKind>();
				IsPrioritySet = allocator.AllocateBoolItem();
				IsTransitionSet = allocator.AllocateBoolItem();
			}
		}
	}
}