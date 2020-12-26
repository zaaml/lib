// <copyright file="VisualStateTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class VisualStateTrigger : DelayStateTriggerBase, IVisualStateListener
	{
		#region Fields

		private string _visualState;

		#endregion

		#region Ctors

		static VisualStateTrigger()
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
				UpdateTriggerState();
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
			if (VisualState == null)
				return;

			IsCurrentState = false;
			GetService<IVisualStateObserver>()?.AttachListener(this);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (VisualStateTrigger) source;

			VisualState = triggerSource.VisualState;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new VisualStateTrigger();
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			DetachVisualStateObserver();
			
			base.DeinitializeTrigger(root);
		}

		private void DetachVisualStateObserver()
		{
			if (VisualState == null) 
				return;

			GetService<IVisualStateObserver>()?.DetachListener(this);

			IsCurrentState = false;
		}

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			AttachVisualStateObserver();
			
			base.InitializeTrigger(root);
		}

		protected override TriggerState UpdateTriggerStateCore()
		{
			return IsCurrentState ? TriggerState.Opened : TriggerState.Closed;
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
				var allocator = GetAllocator<VisualStateTrigger>();

				IsCurrentState = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}