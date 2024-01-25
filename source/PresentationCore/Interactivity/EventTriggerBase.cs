// <copyright file="EventTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class EventTriggerBase : ActionSourceTriggerBase, INotifyPropertyChanged
	{
		private object _eventArgs;
		private event PropertyChangedEventHandler PropertyChangedInt;

		internal EventTriggerBase()
		{
		}

		public object EventArgs
		{
			get => _eventArgs;
			private set
			{
				_eventArgs = value;

				OnPropertyChanged(nameof(EventArgs));
			}
		}

		protected TriggerRuntimeBase TriggerRuntime { get; private set; }

		protected abstract TriggerRuntimeBase CreateTriggerRuntime();

		protected void DeinitializeRuntime()
		{
			TriggerRuntime = TriggerRuntime.DisposeExchange();
		}

		protected void InitializeRuntime()
		{
			TriggerRuntime = TriggerRuntime.DisposeExchange();

			if (ActualSource == null)
				return;

			TriggerRuntime = CreateTriggerRuntime();
		}

		internal sealed override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			InitializeRuntime();
		}

		protected sealed override void OnActualSourceChanged(DependencyObject oldSource)
		{
			InitializeRuntime();
		}

		protected void OnEvent(object sender, object eventArgs)
		{
			EventArgs = eventArgs;

			foreach (var action in ActualActions.OfType<IEventTriggerArgsSupport>())
				action.SetArgs(eventArgs as EventArgs);

			Invoke();

			EventArgs = null;

			foreach (var action in ActualActions.OfType<IEventTriggerArgsSupport>())
				action.SetArgs(null);
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedInt?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		internal sealed override void UnloadCore(IInteractivityRoot root)
		{
			DeinitializeRuntime();

			base.UnloadCore(root);
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add => PropertyChangedInt += value;
			remove => PropertyChangedInt -= value;
		}

		protected abstract class TriggerRuntimeBase : IDisposable
		{
			private readonly WeakReference _weakTriggerReference;

			protected TriggerRuntimeBase(EventTriggerBase trigger)
			{
				ActualSource = trigger.ActualSource;
				_weakTriggerReference = new WeakReference(trigger);
			}

			protected DependencyObject ActualSource { get; private set; }

			protected EventTriggerBase Trigger => (EventTriggerBase)_weakTriggerReference.Target;

			public abstract void DisposeCore();

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