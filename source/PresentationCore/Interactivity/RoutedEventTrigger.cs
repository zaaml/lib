// <copyright file="RoutedEventTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class RoutedEventTrigger : EventTriggerBase
	{
		#region Fields

		private RoutedEvent _event;

		#endregion

		#region Ctors

		static RoutedEventTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		#endregion

		#region Properties

		public RoutedEvent Event
		{
			get => _event;
			set
			{
				if (ReferenceEquals(Event, value))
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				_event = value;

				if (IsLoaded)
					InitializeRuntime();
			}
		}

		public bool HandledEventsToo
		{
			get => PackedDefinition.HandledEventsToo.GetValue(PackedValue);
			set
			{
				if (HandledEventsToo == value)
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				PackedDefinition.HandledEventsToo.SetValue(ref PackedValue, value);

				if (IsLoaded)
					InitializeRuntime();
			}
		}

		public bool MarkHandled
		{
			get => PackedDefinition.MarkHandled.GetValue(PackedValue);
			set
			{
				if (MarkHandled == value)
					return;

				PackedDefinition.MarkHandled.SetValue(ref PackedValue, value);

				var routedEventRuntime = (RoutedEventTriggerRuntime) TriggerRuntime;
				if (routedEventRuntime != null)
					routedEventRuntime.MarkHandled = value;
			}
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (RoutedEventTrigger) source;

			Event = triggerSource.Event;
			HandledEventsToo = triggerSource.HandledEventsToo;
			MarkHandled = triggerSource.MarkHandled;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new RoutedEventTrigger();
		}

		protected override TriggerRuntimeBase CreateTriggerRuntime()
		{
			return new RoutedEventTriggerRuntime(this)
			{
				MarkHandled = MarkHandled
			};
		}

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition HandledEventsToo;
			public static readonly PackedBoolItemDefinition MarkHandled;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<RoutedEventTrigger>();

				HandledEventsToo = allocator.AllocateBoolItem();
				MarkHandled = allocator.AllocateBoolItem();
			}

			#endregion
		}

		private class RoutedEventTriggerRuntime : TriggerRuntimeBase
		{
			#region Fields

			private readonly Delegate _eventHandler;
			private readonly RoutedEvent _routedEvent;

			#endregion

			#region Ctors

			public RoutedEventTriggerRuntime(RoutedEventTrigger trigger) : base(trigger)
			{
				_routedEvent = trigger.Event;
				_eventHandler = RoutedEventHandlerUtils.CreateRoutedEventHandler(_routedEvent, OnRoutedEvent);

				(ActualSource as UIElement)?.AddHandler(_routedEvent, _eventHandler, trigger.HandledEventsToo);
			}

			#endregion

			#region Properties

			public bool MarkHandled { private get; set; }

			#endregion

			#region  Methods

			public override void DisposeCore()
			{
				(ActualSource as UIElement)?.RemoveHandler(_routedEvent, _eventHandler);
			}

			private void OnRoutedEvent(object sender, object args)
			{
				((RoutedEventTrigger) Trigger)?.OnEvent(sender, (EventArgs) args);

				if (MarkHandled)
					RoutedEventArgsUtils.SetHandled((RoutedEventArgs) args, true);
			}

			#endregion
		}

		#endregion
	}
}