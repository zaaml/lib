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
		#region Static Fields and Constants

		private static readonly uint DefaultPackedValue;

		#endregion

		#region Ctors

		static SetterBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.IsEnabled.SetValue(ref DefaultPackedValue, true);
		}

		protected SetterBase()
		{
			PackedValue |= DefaultPackedValue;
		}

		#endregion

		#region Properties

	  internal virtual SetterBase ActualSetter => this;

		internal virtual DependencyProperty DependencyProperty  => DependencyPropertyManager.UnresolvedDependencyProperty;

    internal virtual DependencyProperty MergeDependencyProperty => DependencyProperty;

	  internal virtual ushort Index
	  {
      get => 0;
      // ReSharper disable once ValueParameterNotUsed
      set { }
	  }

		internal bool IsApplied => Status == StatusKindConst.Applied;

		protected bool IsAppliedOrQueried
		{
			get
			{
				var status = Status;
				return status == StatusKindConst.Applied || status == StatusKindConst.ApplyQueried;
			}
		}

		protected bool IsApplyQueried => Status == StatusKindConst.ApplyQueried;

		internal bool IsEnabled
		{
			get => PackedDefinition.IsEnabled.GetValue(PackedValue);
			set
			{
				if (IsEnabled == value)
					return;

				PackedDefinition.IsEnabled.SetValue(ref PackedValue, value);
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

		private uint Status
		{
			get => PackedDefinition.Status.GetValue(PackedValue);
			set => PackedDefinition.Status.SetValue(ref PackedValue, value);
		}

		#endregion

		#region  Methods

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

		#endregion

		#region  Nested Types

		private static class StatusKindConst
		{
			#region Static Fields and Constants

			public const uint Default = 0;
			public const uint ApplyQueried = 1;
			public const uint Applied = 2;
			public const uint Overriden = 3;

			#endregion
		}

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedUIntItemDefinition Status;
			public static readonly PackedBoolItemDefinition IsEnabled;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<SetterBase>();

				Status = allocator.AllocateUIntItem(3);
				IsEnabled = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}