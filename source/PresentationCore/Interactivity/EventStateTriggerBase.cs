// <copyright file="EventStateTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class EventStateTriggerBase : SourceTriggerBase
	{
		static EventStateTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		internal EventStateTriggerBase()
		{
		}

		private bool IsOpen
		{
			get => PackedDefinition.IsOpen.GetValue(PackedValue);
			set
			{
				if (value == IsOpen)
					return;

				PackedDefinition.IsOpen.SetValue(ref PackedValue, value);

				UpdateTriggerState();
			}
		}

		protected TriggerRuntimeBase TriggerRuntime { get; private set; }

		protected abstract TriggerRuntimeBase CreateTriggerRuntime();

		protected void DeinitializeRuntime()
		{
			TriggerRuntime = TriggerRuntime.DisposeExchange();
		}

		internal sealed override void DeinitializeTrigger(IInteractivityRoot root)
		{
			DeinitializeRuntime();

			base.DeinitializeTrigger(root);
		}

		protected void InitializeRuntime()
		{
			TriggerRuntime = TriggerRuntime.DisposeExchange();

			if (ActualSource == null)
				return;

			TriggerRuntime = CreateTriggerRuntime();
		}

		internal sealed override void InitializeTrigger(IInteractivityRoot root)
		{
			InitializeRuntime();

			base.InitializeTrigger(root);
		}

		protected sealed override void OnActualSourceChanged(DependencyObject oldSource)
		{
			base.OnActualSourceChanged(oldSource);


			InitializeRuntime();
		}

		protected void OnCloseEvent(object sender, EventArgs args)
		{
			IsOpen = false;
		}

		protected void OnOpenEvent(object sender, EventArgs args)
		{
			IsOpen = true;
		}

		protected sealed override TriggerState UpdateTriggerStateCore()
		{
			return IsOpen ? TriggerState.Opened : TriggerState.Closed;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsOpen;

			static PackedDefinition()
			{
				var allocator = GetAllocator<EventStateTriggerBase>();

				IsOpen = allocator.AllocateBoolItem();
			}
		}

		protected abstract class TriggerRuntimeBase : IDisposable
		{
			private readonly WeakReference _weakTriggerReference;

			protected TriggerRuntimeBase(EventStateTriggerBase trigger)
			{
				ActualSource = trigger.ActualSource;

				_weakTriggerReference = new WeakReference(trigger);
			}

			protected DependencyObject ActualSource { get; private set; }

			protected EventStateTriggerBase Trigger => (EventStateTriggerBase)_weakTriggerReference.Target;

			protected abstract void DisposeCore();

			public void Dispose()
			{
				if (ActualSource == null)
					return;

				DisposeCore();

				ActualSource = null;
			}
		}
	}
}