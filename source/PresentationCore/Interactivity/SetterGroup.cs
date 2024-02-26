// <copyright file="SetterGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Animation;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("Setters")]
	public sealed class SetterGroup : PropertyValueSetter
	{
		private SetterCollection _setters;

		internal IEnumerable<PropertyValueSetter> ActualPropertySetters => ActualSetters.Select(s => s.ActualSetter).OfType<PropertyValueSetter>();

		internal IEnumerable<SetterBase> ActualSetters => _setters ?? Enumerable.Empty<SetterBase>();

		internal override IEnumerable<InteractivityObject> Children => _setters == null ? base.Children : base.Children.Concat(_setters);

		public SetterCollection Setters => _setters ??= CreateSetterCollection();

		public SetterCollection SettersSource
		{
			// TODO Inspect this
			get => _setters?.CloneParent;
			set
			{
				if (ReferenceEquals(_setters, value))
					return;

				IInteractivityRoot root = null;

				var isLoaded = IsLoaded;

				if (_setters != null)
					if (isLoaded)
						_setters.Unload(root = Root);

				_setters = value?.DeepCloneCollection<SetterCollection, SetterBase>(this);

				if (_setters != null)
					if (isLoaded)
						_setters.Load(root);
			}
		}

		protected override bool ApplyCore()
		{
			foreach (var setter in ActualSetters)
				setter.Apply();

			return true;
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var groupSetter = (SetterGroup)source;

			_setters = groupSetter._setters?.DeepCloneCollection<SetterCollection, SetterBase>(this);

			SetterValueResolver.CopyFrom(this, groupSetter);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new SetterGroup();
		}

		private SetterCollection CreateSetterCollection()
		{
			return new SetterCollection(this);
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			_setters?.Load(root);
		}

		protected override void OnActualPriorityChanged(short oldPriority, short newPriority)
		{
			base.OnActualPriorityChanged(oldPriority, newPriority);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualPriorityChanged(oldPriority, newPriority);
		}

		protected override void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			base.OnActualPropertyChanged(oldProperty, newProperty);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualPropertyChanged(oldProperty, newProperty);
		}

		protected override void OnActualTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
		{
			base.OnActualTargetChanged(oldTarget, newTarget);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualTargetChanged(oldTarget, newTarget);
		}

		protected override void OnActualTransitionChanged(Transition oldTransition, Transition newTransition)
		{
			base.OnActualTransitionChanged(oldTransition, newTransition);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualTransitionChanged(oldTransition, newTransition);
		}

		protected override void OnActualValueChanged(object oldValue, object newValue)
		{
			base.OnActualValueChanged(oldValue, newValue);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualValueChanged(oldValue, newValue);
		}

		protected override void OnActualValuePathChanged(string oldValuePath, string newValuePath)
		{
			base.OnActualValuePathChanged(oldValuePath, newValuePath);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualValuePathChanged(oldValuePath, newValuePath);
		}

		protected override void OnActualValuePathSourceChanged(ValuePathSource oldValuePathSource, ValuePathSource newValuePathSource)
		{
			base.OnActualValuePathSourceChanged(oldValuePathSource, newValuePathSource);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualValuePathSourceChanged(oldValuePathSource, newValuePathSource);
		}

		protected override void OnActualVisualStateTriggerChanged(string oldVisualStateTrigger, string newVisualStateTrigger)
		{
			base.OnActualVisualStateTriggerChanged(oldVisualStateTrigger, newVisualStateTrigger);

			foreach (var setter in ActualPropertySetters)
				setter.OnParentActualVisualStateTriggerChanged(oldVisualStateTrigger, newVisualStateTrigger);
		}

		protected override void OnActualClassTriggerChanged(string oldClassTrigger, string newClassTrigger)
		{
			base.OnActualClassTriggerChanged(oldClassTrigger, newClassTrigger);

			foreach (var setter in ActualPropertySetters) 
				setter.OnParentActualClassTriggerChanged(oldClassTrigger, newClassTrigger);
		}

		protected override void UndoCore()
		{
			foreach (var setter in ActualSetters)
				setter.Undo();
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			_setters?.Unload(root);

			base.UnloadCore(root);
		}
	}
}