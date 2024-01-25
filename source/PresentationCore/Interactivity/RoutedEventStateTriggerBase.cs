// <copyright file="RoutedEventStateTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class RoutedEventStateTriggerBase : EventStateTriggerBase
	{
		static RoutedEventStateTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		protected abstract RoutedEvent CloseEventCore { get; }

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

				var routedEventRuntime = (RoutedEventTriggerRuntime)TriggerRuntime;
				if (routedEventRuntime != null)
					routedEventRuntime.MarkHandled = value;
			}
		}

		protected abstract RoutedEvent OpenEventCore { get; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (RoutedEventStateTrigger)source;

			HandledEventsToo = triggerSource.HandledEventsToo;
			MarkHandled = triggerSource.MarkHandled;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new RoutedEventStateTrigger();
		}

		protected sealed override TriggerRuntimeBase CreateTriggerRuntime()
		{
			return new RoutedEventTriggerRuntime(this)
			{
				MarkHandled = MarkHandled
			};
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition HandledEventsToo;
			public static readonly PackedBoolItemDefinition MarkHandled;

			static PackedDefinition()
			{
				var allocator = GetAllocator<RoutedEventStateTrigger>();
				HandledEventsToo = allocator.AllocateBoolItem();
				MarkHandled = allocator.AllocateBoolItem();
			}
		}

		private class RoutedEventTriggerRuntime : TriggerRuntimeBase
		{
			private readonly RoutedEvent _closeEvent;
			private readonly Delegate _closeEventHandler;
			private readonly RoutedEvent _openEvent;
			private readonly Delegate _openEventHandler;

			public RoutedEventTriggerRuntime(RoutedEventStateTriggerBase trigger) : base(trigger)
			{
				var uiElement = ActualSource as UIElement;

				if (uiElement == null)
					return;

				_openEvent = trigger.OpenEventCore;
				_closeEvent = trigger.CloseEventCore;

				_openEventHandler = RoutedEventHandlerUtils.CreateRoutedEventHandler(_openEvent, OnOpenRoutedEvent);
				_closeEventHandler = RoutedEventHandlerUtils.CreateRoutedEventHandler(_openEvent, OnCloseRoutedEvent);

				uiElement.AddHandler(_openEvent, _openEventHandler, trigger.HandledEventsToo);
				uiElement.AddHandler(_closeEvent, _closeEventHandler, trigger.HandledEventsToo);
			}

			public bool MarkHandled { get; set; }

			protected override void DisposeCore()
			{
				if (ActualSource is not UIElement uiElement)
					return;

				uiElement.RemoveHandler(_openEvent, _openEventHandler);
				uiElement.RemoveHandler(_closeEvent, _closeEventHandler);
			}

			private void OnCloseRoutedEvent(object sender, object args)
			{
				var trigger = ((RoutedEventStateTriggerBase)Trigger);

				trigger?.OnCloseEvent(sender, (EventArgs)args);

				if (MarkHandled)
					RoutedEventArgsUtils.SetHandled((RoutedEventArgs)args, true);
			}

			private void OnOpenRoutedEvent(object sender, object args)
			{
				var trigger = ((RoutedEventStateTriggerBase)Trigger);

				trigger?.OnOpenEvent(sender, (EventArgs)args);

				if (MarkHandled)
					RoutedEventArgsUtils.SetHandled((RoutedEventArgs)args, true);
			}
		}
	}
}