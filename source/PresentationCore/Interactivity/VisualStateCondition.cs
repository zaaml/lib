// <copyright file="VisualStateCondition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class VisualStateCondition : ConditionBase, IVisualStateListener
	{
		#region Fields

		private string _visualState;

		#endregion

		#region Ctors

		static VisualStateCondition()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		#endregion

		#region Properties

		private bool IsCurrentState
		{
			get => PackedDefinition.IsCurrentState.GetValue(PackedValue);
			set
			{
				if (IsCurrentState == value)
					return;

				PackedDefinition.IsCurrentState.SetValue(ref PackedValue, value);
				UpdateConditionState();
			}
		}

		public string VisualState
		{
			get => _visualState;
			set
			{
				if (_visualState == value)
					return;

				if (IsLoaded)
					DetachVisualStateObserver();

				_visualState = value;

				if (IsLoaded)
					AttachVisualStateObserver();
			}
		}

		#endregion

		#region  Methods

		private void AttachVisualStateObserver()
		{
			if (VisualState == null) return;

			IsCurrentState = false;
			GetService<IVisualStateObserver>()?.AttachListener(this);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var conditionSource = (VisualStateCondition) source;

			VisualState = conditionSource.VisualState;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new VisualStateCondition();
		}

		internal override void Deinitialize(IInteractivityRoot root)
		{
			DetachVisualStateObserver();
			base.Deinitialize(root);
		}

		private void DetachVisualStateObserver()
		{
			if (VisualState == null) return;

			GetService<IVisualStateObserver>()?.DetachListener(this);

			IsCurrentState = false;
		}

		internal override void Initialize(IInteractivityRoot root)
		{
			AttachVisualStateObserver();
			base.Initialize(root);
		}

		protected override bool UpdateConditionStateCore()
		{
			return IsCurrentState;
		}

		#endregion

		#region Interface Implementations

		#region IVisualStateListener

		string IVisualStateListener.VisualStateName => VisualState;

		void IVisualStateListener.EnterState(bool useTransitions)
		{
			IsCurrentState = true;
		}

		void IVisualStateListener.LeaveState(bool useTransitions)
		{
			IsCurrentState = false;
		}

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsCurrentState;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<VisualStateCondition>();

				IsCurrentState = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}