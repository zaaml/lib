// <copyright file="SetterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class SetterBase : InteractivityObject
	{
		private static readonly uint DefaultPackedValue;

		static SetterBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.IsVisualStateTriggerEnabled.SetValue(ref DefaultPackedValue, true);
			PackedDefinition.IsClassTriggerEnabled.SetValue(ref DefaultPackedValue, true);
		}

		protected SetterBase()
		{
			PackedValue |= DefaultPackedValue;
		}

		internal virtual SetterBase ActualSetter => this;

		internal virtual DependencyProperty DependencyProperty => DependencyPropertyManager.UnresolvedDependencyProperty;

		internal virtual ushort Index
		{
			get => 0;
			// ReSharper disable once ValueParameterNotUsed
			set { }
		}

		internal bool IsApplied => Status == StatusKindConst.Applied;

		protected bool IsAppliedOrQueried => Status is StatusKindConst.Applied or StatusKindConst.ApplyQueried;

		protected bool IsApplyQueried => Status == StatusKindConst.ApplyQueried;

		internal bool IsEnabled => IsVisualStateTriggerEnabled && IsClassTriggerEnabled;

		internal bool IsVisualStateTriggerEnabled
		{
			get => PackedDefinition.IsVisualStateTriggerEnabled.GetValue(PackedValue);
			set
			{
				if (IsVisualStateTriggerEnabled == value)
					return;

				PackedDefinition.IsVisualStateTriggerEnabled.SetValue(ref PackedValue, value);
				OnIsEnabledIntChanged();
			}
		}

		internal bool IsClassTriggerEnabled
		{
			get => PackedDefinition.IsClassTriggerEnabled.GetValue(PackedValue);
			set
			{
				if (IsClassTriggerEnabled == value)
					return;

				PackedDefinition.IsClassTriggerEnabled.SetValue(ref PackedValue, value);
				OnIsEnabledIntChanged();
			}
		}

		internal bool IsOverriden
		{
			get => Status == StatusKindConst.Overriden;
			set
			{
				if (IsAppliedOrQueried)
					throw new InvalidOperationException();

				Status = value ? StatusKindConst.Overriden : StatusKindConst.Default;
			}
		}

		internal virtual DependencyProperty MergeDependencyProperty => DependencyProperty;

		private uint Status
		{
			get => PackedDefinition.Status.GetValue(PackedValue);
			set => PackedDefinition.Status.SetValue(ref PackedValue, value);
		}

		internal void Apply()
		{
			if (IsOverriden)
				return;

			var isApplied = false;

			try
			{
				if (IsLoaded == false)
					return;

				if (IsEnabled == false)
					return;

				isApplied = ApplyCore();
			}
			finally
			{
				Status = isApplied ? StatusKindConst.Applied : StatusKindConst.ApplyQueried;
			}
		}

		protected abstract bool ApplyCore();

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			if (IsApplyQueried)
				Apply();
		}
		
		protected virtual void OnIsEnabledIntChanged()
		{
			UpdateState();
		}

		internal abstract DependencyProperty ResolveProperty(Type targetType);

		internal void Undo()
		{
			if (IsOverriden)
				return;

			try
			{
				if (IsLoaded == false)
					return;

				if (IsEnabled == false)
					return;

				if (IsApplied == false)
					return;

				UndoCore();
			}
			finally
			{
				Status = StatusKindConst.Default;
			}
		}

		protected abstract void UndoCore();

		internal override void UnloadCore(IInteractivityRoot root)
		{
			if (IsApplied)
				Undo();

			base.UnloadCore(root);
		}

		private void UpdateState()
		{
			if (IsOverriden)
				return;

			if (IsEnabled == false)
			{
				if (IsApplied == false) return;

				UndoCore();

				Status = StatusKindConst.ApplyQueried;
			}
			else
			{
				if (IsApplyQueried == false)
					return;

				Status = ApplyCore() ? StatusKindConst.Applied : StatusKindConst.ApplyQueried;
			}
		}

		private protected override bool TryProvideValue(object target, object targetProperty, IServiceProvider serviceProvider, out object value)
		{
			if (target is not FrameworkElement frameworkElement || targetProperty is not DependencyProperty dependencyProperty) 
				return base.TryProvideValue(target, targetProperty, serviceProvider, out value);
			
			var setters = Extension.GetSetters(frameworkElement);

			setters.Add(this);

			value = frameworkElement.GetValue(dependencyProperty);

			return true;
		}

		private static class StatusKindConst
		{
			public const uint Default = 0;
			public const uint ApplyQueried = 1;
			public const uint Applied = 2;
			public const uint Overriden = 3;
		}

		private static class PackedDefinition
		{
			public static readonly PackedUIntItemDefinition Status;
			public static readonly PackedBoolItemDefinition IsVisualStateTriggerEnabled;
			public static readonly PackedBoolItemDefinition IsClassTriggerEnabled;

			static PackedDefinition()
			{
				var allocator = GetAllocator<SetterBase>();

				Status = allocator.AllocateUIntItem(3);
				IsVisualStateTriggerEnabled = allocator.AllocateBoolItem();
				IsClassTriggerEnabled = allocator.AllocateBoolItem();
			}
		}
	}
}