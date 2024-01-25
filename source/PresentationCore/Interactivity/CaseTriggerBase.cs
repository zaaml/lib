// <copyright file="CaseTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class CaseTriggerBase : StateTriggerBase
	{
		static CaseTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		internal bool IsOpen
		{
			get => PackedDefinition.IsOpen.GetValue(PackedValue);
			set
			{
				if (IsOpen == value)
					return;

				PackedDefinition.IsOpen.SetValue(ref PackedValue, value);
				UpdateTriggerState();
			}
		}

		internal SwitchTriggerBase SwitchDataTrigger => Parent as SwitchTriggerBase;

		protected sealed override TriggerState UpdateTriggerStateCore()
		{
			return SwitchDataTrigger != null && IsOpen ? TriggerState.Opened : TriggerState.Closed;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsOpen;

			static PackedDefinition()
			{
				var allocator = GetAllocator<CaseTriggerBase>();

				IsOpen = allocator.AllocateBoolItem();
			}
		}
	}
}