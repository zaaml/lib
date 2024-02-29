// <copyright file="ClassTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class ClassTrigger : StateTriggerBase
	{
		private string _class;

		static ClassTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		public string Class
		{
			get => _class;
			set
			{
				if (_class == value)
					return;

				if (IsLoaded)
					UpdateCurrentState();

				_class = value;

				if (IsLoaded)
					UpdateCurrentState();
			}
		}

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

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (ClassTrigger)source;

			Class = triggerSource.Class;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new ClassTrigger();
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			base.DeinitializeTrigger(root);

			UpdateCurrentState();
		}

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			base.InitializeTrigger(root);

			UpdateCurrentState();
		}

		internal void OnClassChangedInternal()
		{
			UpdateCurrentState();
		}

		private void UpdateCurrentState()
		{
			var interactivityTarget = InteractivityTarget;

			IsCurrentState = interactivityTarget != null && Extension.GetActualClass(interactivityTarget)?.HasClass(Class) == true;
		}

		protected override TriggerState UpdateTriggerStateCore()
		{
			return IsCurrentState ? TriggerState.Opened : TriggerState.Closed;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsCurrentState;

			static PackedDefinition()
			{
				var allocator = GetAllocator<VisualStateTrigger>();

				IsCurrentState = allocator.AllocateBoolItem();
			}
		}
	}
}