// <copyright file="EventTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using Zaaml.Core;
using Zaaml.Core.Reflection;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class EventTrigger : EventTriggerBase
	{
		private string _event;

		public string Event
		{
			get => _event;
			set
			{
				if (string.Equals(Event, value))
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				_event = value;

				if (IsLoaded)
					InitializeRuntime();
			}
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (EventTrigger)source;

			Event = triggerSource.Event;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new EventTrigger();
		}

		protected override TriggerRuntimeBase CreateTriggerRuntime()
		{
			return new EventTriggerRuntime(this);
		}

		private class EventTriggerRuntime : TriggerRuntimeBase
		{
			private readonly Delegate _delegate;
			private readonly EventInfo _eventInfo;

			public EventTriggerRuntime(EventTrigger trigger) : base(trigger)
			{
				var actualSource = ActualSource;
				var sourceType = actualSource.GetType();
				var eventName = trigger.Event;

				if (string.IsNullOrEmpty(eventName))
					return;

				_eventInfo = sourceType.GetEvent(trigger.Event, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

				if (_eventInfo == null)
				{
					LogService.LogWarning($"Can not find event {trigger.Event} in Source {actualSource} or type {sourceType}");

					return;
				}

				_delegate = _eventInfo.CreateDelegate(OnEvent);

				_eventInfo.AddEventHandler(actualSource, _delegate);
			}

			public override void DisposeCore()
			{
				_eventInfo.RemoveEventHandler(ActualSource, _delegate);
			}

			private void OnEvent(object sender, object args)
			{
				((EventTrigger)Trigger)?.OnEvent(sender, (EventArgs)args);
			}
		}
	}
}