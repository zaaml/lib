// <copyright file="CaseTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class CaseTriggerBase : DelayStateTriggerBase
	{
		#region Ctors

		static CaseTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

		protected sealed override TriggerState UpdateTriggerStateCore()
		{
			return SwitchDataTrigger != null && IsOpen ? TriggerState.Opened : TriggerState.Closed;
		}

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsOpen;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<CaseTriggerBase>();

				IsOpen = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}