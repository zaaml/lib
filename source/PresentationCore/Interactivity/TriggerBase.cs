// <copyright file="TriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TriggerBase : InteractivityObject
	{
		#region Ctors

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

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

		protected virtual void OnIsEnabledChanged()
		{
		}

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsEnabled;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<TriggerBase>();

				IsEnabled = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}