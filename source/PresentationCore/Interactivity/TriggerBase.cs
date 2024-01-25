// <copyright file="TriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TriggerBase : InteractivityObject
	{
		private static readonly uint DefaultPackedValue;

		static TriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.IsEnabled.SetValue(ref DefaultPackedValue, false);
		}

		protected TriggerBase()
		{
			PackedValue |= DefaultPackedValue;
		}

		protected internal bool IsEnabled
		{
			get => PackedDefinition.IsEnabled.GetValue(PackedValue);
			set
			{
				if (IsEnabled == value)
					return;

				PackedDefinition.IsEnabled.SetValue(ref PackedValue, value);

				OnIsEnabledChanged();
			}
		}

		protected virtual void OnIsEnabledChanged()
		{
		}

		private protected override bool TryProvideValue(object target, object targetProperty, IServiceProvider serviceProvider, out object value)
		{
			if (target is not FrameworkElement frameworkElement) 
				return base.TryProvideValue(target, targetProperty, serviceProvider, out value);
			
			var triggers = Extension.GetTriggers(frameworkElement);

			triggers.Add(this);

			value = triggers;

			return true;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsEnabled;

			static PackedDefinition()
			{
				var allocator = GetAllocator<TriggerBase>();

				IsEnabled = allocator.AllocateBoolItem();
			}
		}
	}
}