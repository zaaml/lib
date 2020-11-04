// <copyright file="RoutedEventStateTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class RoutedEventStateTrigger : EventStateTriggerBase
	{
		#region Fields

		private RoutedEvent _closeEvent;
		private RoutedEvent _openEvent;

		#endregion

		#region Ctors

		static RoutedEventStateTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		#endregion

		#region Properties

		public RoutedEvent CloseEvent
		{
			get => _closeEvent;
			set
			{
				if (ReferenceEquals(CloseEvent, value))
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				_closeEvent = value;

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

		public RoutedEvent OpenEvent
		{
			get => _openEvent;
			set
			{
				if (ReferenceEquals(OpenEvent, value))
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				_openEvent = value;

				if (IsLoaded)
					InitializeRuntime();
			}
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (RoutedEventStateTrigger) source;

			CloseEvent = triggerSource.CloseEvent;
			OpenEvent = triggerSource.OpenEvent;
			HandledEventsToo = triggerSource.HandledEventsToo;
			MarkHandled = triggerSource.MarkHandled;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new RoutedEventStateTrigger();
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
				var allocator = GetAllocator<RoutedEventStateTrigger>();
				HandledEventsToo = allocator.AllocateBoolItem();
				MarkHandled = allocator.AllocateBoolItem();
			}

			#endregion
		}

		private class RoutedEventTriggerRuntime : TriggerRuntimeBase
		{
			#region Fields

			private readonly RoutedEvent _closeEvent;
			private readonly Delegate _closeEventHandler;
			private readonly RoutedEvent _openEvent;
			private readonly Delegate _openEventHandler;

			#endregion

			#region Ctors

			public RoutedEventTriggerRuntime(RoutedEventStateTrigger trigger) : base(trigger)
			{
				var uiElement = ActualSource as UIElement;

				if (uiElement == null)
					return;

				_openEvent = trigger.OpenEvent;
				_closeEvent = trigger.CloseEvent;

				_openEventHandler = RoutedEventHandlerUtils.CreateRoutedEventHandler(_openEvent, OnOpenRoutedEvent);
				_closeEventHandler = RoutedEventHandlerUtils.CreateRoutedEventHandler(_openEvent, OnCloseRoutedEvent);

				uiElement.AddHandler(_openEvent, _openEventHandler, trigger.HandledEventsToo);
				uiElement.AddHandler(_closeEvent, _closeEventHandler, trigger.HandledEventsToo);
			}

			#endregion

			#region Properties

			public bool MarkHandled { get; set; }

			#endregion

			#region  Methods

			protected override void DisposeCore()
			{
				var uiElement = ActualSource as UIElement;

				if (uiElement == null)
					return;

				uiElement.RemoveHandler(_openEvent, _openEventHandler);
				uiElement.RemoveHandler(_closeEvent, _closeEventHandler);
			}

			private void OnCloseRoutedEvent(object sender, object args)
			{
				((RoutedEventStateTrigger) Trigger)?.OnCloseEvent(sender, (EventArgs) args);

				if (MarkHandled)
					RoutedEventArgsUtils.SetHandled((RoutedEventArgs) args, true);
			}

			private void OnOpenRoutedEvent(object sender, object args)
			{
				((RoutedEventStateTrigger) Trigger)?.OnOpenEvent(sender, (EventArgs) args);

				if (MarkHandled)
					RoutedEventArgsUtils.SetHandled((RoutedEventArgs) args, true);
			}

			#endregion
		}

		#endregion
	}
}