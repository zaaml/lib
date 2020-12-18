// <copyright file="PulseTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty(nameof(Setters))]
	public sealed class PulseTrigger : TriggerActionBase
	{
		#region Properties

		private PulseStateTrigger ActualTrigger => Trigger ??= new PulseStateTrigger(this);

		public TriggerActionCollection EnterActions => ActualTrigger.EnterActions;

		public TriggerActionCollection ExitActions => ActualTrigger.ExitActions;

		public Duration OpenDelay
		{
			get => Trigger?.OpenDelay ?? TimeSpan.Zero;
			set
			{
				if (OpenDelay != value)
					ActualTrigger.OpenDelay = value;
			}
		}

		public PulseTriggerBehavior PulseBehavior
		{
			get => Trigger?.PulseBehavior ?? PulseTriggerBehavior.Default;
			set => ActualTrigger.PulseBehavior = value;
		}

		public SetterCollectionBase Setters => ActualTrigger.Setters;

		public Duration SustainDelay
		{
			get => Trigger?.CloseDelay ?? TimeSpan.Zero;
			set
			{
				if (SustainDelay != value)
					ActualTrigger.CloseDelay = value;
			}
		}

		private PulseStateTrigger Trigger { get; set; }

		public TriggerCollectionBase Triggers => ActualTrigger.Triggers;

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourcePulseTrigger = (PulseTrigger) source;

			Trigger = (PulseStateTrigger) sourcePulseTrigger.Trigger?.DeepClone();
		}

		protected override InteractivityObject CreateInstance()
		{
			return new PulseTrigger();
		}


		protected override void InvokeCore()
		{
			Trigger?.Pulse();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Trigger?.Load(root);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Trigger?.Unload(root);

			base.UnloadCore(root);
		}

		#endregion

		#region  Nested Types

		private sealed class PulseStateTrigger : DelayStateTriggerBase
		{
			#region Ctors

			static PulseStateTrigger()
			{
				RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
			}

			public PulseStateTrigger(InteractivityObject pulseTrigger)
			{
				Parent = pulseTrigger;

				IsEnabled = true;

				if (pulseTrigger.IsLoaded)
					Load();
			}

			#endregion

			#region Properties

			public PulseTriggerBehavior PulseBehavior
			{
				get => PackedDefinition.PulseBehavior.GetValue(PackedValue);
				set => PackedDefinition.PulseBehavior.SetValue(ref PackedValue, value);
			}

			private StateKind State
			{
				get => PackedDefinition.IsPulsing.GetValue(PackedValue);
				set
				{
					if (State == value)
						return;

					PackedDefinition.IsPulsing.SetValue(ref PackedValue, value);

					UpdateTriggerState();
				}
			}

			#endregion

			#region  Methods

			protected internal override void CopyMembersOverride(InteractivityObject source)
			{
				base.CopyMembersOverride(source);

				var sourcePulseStateTrigger = (PulseStateTrigger) source;

				PulseBehavior = sourcePulseStateTrigger.PulseBehavior;
			}

			protected override InteractivityObject CreateInstance()
			{
				throw new NotSupportedException();
			}

			protected override void OnClosedCore()
			{
				State = StateKind.Closed;
			}

			protected override void OnOpenedCore()
			{
				State = StateKind.Closing;
				ActualDelayTrigger.InvokeClose();
			}

			public void Pulse()
			{
				if (State == StateKind.Closed)
				{
					State = StateKind.Opening;
					ActualDelayTrigger.InvokeOpen();
				}
				else if (State == StateKind.Opening)
				{
					if ((PulseBehavior & PulseTriggerBehavior.KeepClosed) != 0)
						ActualDelayTrigger.InvokeOpen();
				}
				else if (State == StateKind.Closing)
				{
					if ((PulseBehavior & PulseTriggerBehavior.KeepOpened) != 0)
						ActualDelayTrigger.InvokeClose();
				}
			}

			protected override TriggerState UpdateTriggerStateCore()
			{
				return State != StateKind.Closed ? TriggerState.Opened : TriggerState.Closed;
			}

			#endregion

			#region  Nested Types

			private enum StateKind
			{
				Closed,
				Opening,
				Closing
			}

			private static class PackedDefinition
			{
				#region Static Fields and Constants

				public static readonly PackedEnumItemDefinition<StateKind> IsPulsing;
				public static readonly PackedEnumItemDefinition<PulseTriggerBehavior> PulseBehavior;

				#endregion

				#region Ctors

				static PackedDefinition()
				{
					var allocator = GetAllocator<PulseStateTrigger>();

					IsPulsing = allocator.AllocateEnumItem<StateKind>();
					PulseBehavior = allocator.AllocateEnumItem<PulseTriggerBehavior>();
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}