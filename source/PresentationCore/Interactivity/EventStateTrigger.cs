// <copyright file="EventStateTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using Zaaml.Core;
using Zaaml.Core.Reflection;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class EventStateTrigger : EventStateTriggerBase
	{
		#region Fields

		private string _closeEvent;
		private string _openEvent;

		#endregion

		#region Properties

		public string CloseEvent
		{
			get => _closeEvent;
			set
			{
				if (string.Equals(CloseEvent, value))
					return;

				if (IsInitialized)
					DeinitializeRuntime();

				_closeEvent = value;

				if (IsInitialized)
					InitializeRuntime();
			}
		}

		public string OpenEvent
		{
			get => _openEvent;
			set
			{
				if (string.Equals(OpenEvent, value))
					return;

				if (IsInitialized)
					DeinitializeRuntime();

				_openEvent = value;

				if (IsInitialized)
					InitializeRuntime();
			}
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (EventStateTrigger) source;

			OpenEvent = triggerSource.OpenEvent;
			CloseEvent = triggerSource.CloseEvent;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new EventStateTrigger();
		}

		protected override TriggerRuntimeBase CreateTriggerRuntime()
		{
			return new EventTriggerRuntime(this);
		}

		#endregion

		#region  Nested Types

		private class EventTriggerRuntime : TriggerRuntimeBase
		{
			#region Fields

			private readonly Delegate _closeDelegate;
			private readonly EventInfo _closeEventInfo;
			private readonly Delegate _openDelegate;
			private readonly EventInfo _openEventInfo;

			#endregion

			#region Ctors

			public EventTriggerRuntime(EventStateTrigger trigger) : base(trigger)
			{
				var actualSource = trigger.ActualSource;
				var sourceType = actualSource.GetType();

				_openEventInfo = sourceType.GetEvent(trigger.OpenEvent);
				_closeEventInfo = sourceType.GetEvent(trigger.CloseEvent);

				if (_openEventInfo == null)
				{
					LogService.LogWarning($"Can not find event {trigger.OpenEvent} in Source {actualSource} or type {sourceType}");

					return;
				}

				if (_closeEventInfo == null)
				{
					LogService.LogWarning($"Can not find event {trigger.CloseEvent} in Source {actualSource} or type {sourceType}");

					return;
				}

				_openDelegate = _openEventInfo.CreateDelegate(OnOpenEvent);
				_closeDelegate = _closeEventInfo.CreateDelegate(OnCloseEvent);

				_openEventInfo.AddEventHandler(actualSource, _openDelegate);
				_closeEventInfo.AddEventHandler(actualSource, _closeDelegate);
			}

			#endregion

			#region  Methods

			protected override void DisposeCore()
			{
				_openEventInfo.RemoveEventHandler(ActualSource, _openDelegate);
				_closeEventInfo.RemoveEventHandler(ActualSource, _closeDelegate);
			}

			private void OnCloseEvent(object sender, object args)
			{
				((EventStateTrigger) Trigger)?.OnCloseEvent(sender, (EventArgs) args);
			}

			private void OnOpenEvent(object sender, object args)
			{
				((EventStateTrigger) Trigger)?.OnOpenEvent(sender, (EventArgs) args);
			}

			#endregion
		}

		#endregion
	}
}