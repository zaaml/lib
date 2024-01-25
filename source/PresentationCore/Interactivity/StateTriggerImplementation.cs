// <copyright file="StateTriggerImplementation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Packed;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.PresentationCore.Interactivity
{
	//TODO implement Invert property for simple != conditions (also for conditions, condition groups, VisualStateTrigger, ...)
	internal class StateTriggerImplementation<TStateTrigger, TStateTriggerImplementation>
		where TStateTrigger : TriggerBase, IStateTrigger<TStateTrigger, TStateTriggerImplementation>
		where TStateTriggerImplementation : StateTriggerImplementation<TStateTrigger, TStateTriggerImplementation>
	{
		private protected static uint DefaultPackedValue;

		private TriggerActionCollection _enterActions;
		private TriggerActionCollection _exitActions;
		private InnerSetterCollection _setters;
		private InnerTriggerCollection _triggers;

		public event EventHandler StateChanged;
		public event EventHandler Opened;
		public event EventHandler Closed;

		protected StateTriggerImplementation(TStateTrigger stateTrigger)
		{
			StateTrigger = stateTrigger;
			StateTrigger.PackedValue |= DefaultPackedValue;
		}

		protected TimeSpan ActualCloseDelay => DelayTrigger?.CloseDelay ?? TimeSpan.Zero;

		public DelayStateTrigger ActualDelayTrigger => DelayTrigger ??= new DelayStateTrigger(OpenTriggerImpl, TimeSpan.Zero, CloseTriggerImpl, TimeSpan.Zero);

		private IEnumerable<TriggerActionBase> ActualEnterActions => _enterActions ?? Enumerable.Empty<TriggerActionBase>();

		private IEnumerable<TriggerActionBase> ActualExitActions => _exitActions ?? Enumerable.Empty<TriggerActionBase>();

		protected TimeSpan ActualOpenDelay => DelayTrigger?.OpenDelay ?? TimeSpan.Zero;

		private IEnumerable<SetterBase> ActualSetters => _setters ?? Enumerable.Empty<SetterBase>();

		private IEnumerable<TriggerBase> ActualTriggers => _triggers ?? Enumerable.Empty<TriggerBase>();

		public IEnumerable<InteractivityObject> Children => StateTrigger.BaseChildren.Concat(ActualEnterActions).Concat(ActualSetters).Concat(ActualExitActions).Concat(ActualTriggers);

		public Duration CloseDelay
		{
			get => DelayTrigger?.CloseDelay ?? default(Duration);
			set
			{
				if (CloseDelay == value)
					return;

				ActualDelayTrigger.CloseDelay = value.HasTimeSpan ? value.TimeSpan : TimeSpan.Zero;
			}
		}

		private DelayStateTrigger DelayTrigger { get; set; }

		private SetterGroup EnsureSourceSetterGroup
		{
			get
			{
				var setterGroup = SourceSetterGroup;

				if (setterGroup != null)
					return setterGroup;

				setterGroup = new SetterGroup { Parent = StateTrigger };

				SourceSetterGroup = setterGroup;

				if (StateTrigger.IsLoaded)
					setterGroup.Load();

				if (IsActuallyOpen)
					setterGroup.Apply();

				return setterGroup;
			}
		}

		public TriggerActionCollection EnterActions => _enterActions ??= new TriggerActionCollection(StateTrigger);

		public TriggerActionCollection ExitActions => _exitActions ??= new TriggerActionCollection(StateTrigger);

		public bool Invert
		{
			get => PackedDefinition.Invert.GetValue(StateTrigger.PackedValue);
			set
			{
				if (Invert == value)
					return;

				PackedDefinition.Invert.SetValue(ref StateTrigger.PackedValue, value);

				UpdateTriggerState();
			}
		}

		public bool IsActuallyOpen
		{
			get => PackedDefinition.IsActuallyOpen.GetValue(StateTrigger.PackedValue);
			private set
			{
				if (IsActuallyOpen == value)
					return;

				PackedDefinition.IsActuallyOpen.SetValue(ref StateTrigger.PackedValue, value);

				if (value)
					OnOpened();
				else
					OnClosed();
			}
		}

		private bool IsClosing
		{
			get => PackedDefinition.IsClosing.GetValue(StateTrigger.PackedValue);
			set => PackedDefinition.IsClosing.SetValue(ref StateTrigger.PackedValue, value);
		}

		public bool IsInitialized => Status >= StateTriggerStatus.Initialized;

		private bool IsOpening
		{
			get => PackedDefinition.IsOpening.GetValue(StateTrigger.PackedValue);
			set => PackedDefinition.IsOpening.SetValue(ref StateTrigger.PackedValue, value);
		}

		public Duration OpenDelay
		{
			get => DelayTrigger?.OpenDelay ?? default(Duration);
			set
			{
				if (OpenDelay == value)
					return;

				ActualDelayTrigger.OpenDelay = value.HasTimeSpan ? value.TimeSpan : TimeSpan.Zero;
			}
		}

		public SetterCollectionBase Setters => _setters ??= new InnerSetterCollection(this);

		public SetterCollection SettersSource
		{
			get => SourceSetterGroup?.SettersSource;
			set
			{
				if (ReferenceEquals(SettersSource, value))
					return;

				EnsureSourceSetterGroup.SettersSource = value;
			}
		}

		private SetterGroup SourceSetterGroup
		{
			get => StateTrigger.MutableData.GetSetValueOrDefault<SetterGroup>();
			set => StateTrigger.MutableData = value;
		}

		public TStateTrigger StateTrigger { get; }

		private StateTriggerStatus Status
		{
			get => PackedDefinition.Status.GetValue(StateTrigger.PackedValue);
			set => PackedDefinition.Status.SetValue(ref StateTrigger.PackedValue, value);
		}

		public TriggerCollectionBase Triggers => _triggers ??= new InnerTriggerCollection(this);

		public TriggerState TriggerState
		{
			get => PackedDefinition.State.GetValue(StateTrigger.PackedValue);
			private set
			{
				var state = TriggerState;

				if (state == value)
					return;

				var prevState = state;

				PackedDefinition.State.SetValue(ref StateTrigger.PackedValue, value);
				OnStateChanged(prevState);
			}
		}

		private TriggerState ApplyInvert(TriggerState state)
		{
			if (Invert == false)
				return state;

			switch (state)
			{
				case TriggerState.Undefined:
					return TriggerState.Undefined;

				case TriggerState.Opened:
					return TriggerState.Closed;

				case TriggerState.Closed:
					return TriggerState.Opened;

				default:
					throw new ArgumentOutOfRangeException(nameof(state));
			}
		}

		private void CloseTrigger()
		{
			if (IsOpening)
			{
				IsOpening = false;

				return;
			}

			CloseTriggerCore();
		}

		public virtual void CloseTriggerCore()
		{
			if (DelayTrigger != null)
			{
				if (IsActuallyOpen)
					DelayTrigger.InvokeClose();
				else
					DelayTrigger.RevokeOpen();
			}
			else
				CloseTriggerImpl();
		}

		private void CloseTriggerImpl()
		{
			IsClosing = true;

			foreach (var trigger in ActualTriggers)
				trigger.IsEnabled = false;

			foreach (var setter in ActualSetters.Reverse())
				setter.Undo();

			SourceSetterGroup?.Undo();

			foreach (var action in ActualExitActions.Reverse())
				action.Invoke();

			// Setter or Action caused trigger open
			if (IsClosing == false && TriggerState == TriggerState.Opened)
			{
				CloseTrigger();

				return;
			}

			IsClosing = false;

			IsActuallyOpen = false;
		}

		public void CopyMembersOverride(TStateTriggerImplementation sourceTrigger)
		{
			_triggers = sourceTrigger._triggers?.DeepCloneCollection<InnerTriggerCollection, TriggerBase>(StateTrigger);
			_setters = sourceTrigger._setters?.DeepCloneCollection<InnerSetterCollection, SetterBase>(StateTrigger);
			_enterActions = sourceTrigger._enterActions?.DeepCloneCollection<TriggerActionCollection, TriggerActionBase>(StateTrigger);
			_exitActions = sourceTrigger._exitActions?.DeepCloneCollection<TriggerActionCollection, TriggerActionBase>(StateTrigger);

			if (sourceTrigger.DelayTrigger != null)
			{
				OpenDelay = sourceTrigger.OpenDelay;
				CloseDelay = sourceTrigger.CloseDelay;
			}

			var settersSource = sourceTrigger.SettersSource;

			if (settersSource != null)
				SettersSource = settersSource;

			PackedDefinition.Invert.SetValue(ref StateTrigger.PackedValue, sourceTrigger.Invert);
		}

		public RuntimeSetter CreateSetter()
		{
			return new TriggerRuntimeSetter();
		}

		internal void DeinitializeTrigger(IInteractivityRoot root)
		{
			StateTrigger.DeinitializeTrigger(root);
		}

		internal void InitializeTrigger(IInteractivityRoot root)
		{
			StateTrigger.InitializeTrigger(root);
		}

		public static void InitPackedDefinition()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.State.SetValue(ref DefaultPackedValue, TriggerState.Undefined);
			PackedDefinition.Status.SetValue(ref DefaultPackedValue, StateTriggerStatus.Default);
		}

		internal void LoadActions(IInteractivityRoot root)
		{
			if (Status != StateTriggerStatus.Initialized)
				return;

			_triggers?.Load(root);
			_enterActions?.Load(root);
			_exitActions?.Load(root);
			_setters?.Load(root);
			SourceSetterGroup?.Load(root);

			Status = StateTriggerStatus.InitializedActions;
		}

		public void LoadCore(IInteractivityRoot root)
		{
			Status = StateTriggerStatus.Initializing;

			InitializeTrigger(root);

			Status = StateTriggerStatus.Initialized;

			UpdateTriggerState();
		}

		private void OnClosed()
		{
			Closed?.Invoke(this, EventArgs.Empty);

			OnClosedCore();
		}

		protected void OnClosedCore()
		{
			StateTrigger.OnClosed();
		}

		public void OnIsEnabledChanged()
		{
			UpdateTriggerState();

			var isEnabled = StateTrigger.IsEnabled;

			if (TriggerState == TriggerState.Opened)
				foreach (var trigger in ActualTriggers)
					trigger.IsEnabled = isEnabled;
			else
				foreach (var trigger in ActualTriggers)
					trigger.IsEnabled = false;
		}

		private void OnOpened()
		{
			OnOpenedCore();

			Opened?.Invoke(this, EventArgs.Empty);
		}

		protected void OnOpenedCore()
		{
			StateTrigger.OnOpened();
		}

		private void OnStateChanged(TriggerState prevState)
		{
			switch (TriggerState)
			{
				case TriggerState.Undefined:
					throw new InvalidOperationException("Trigger can not go to undefined state");

				case TriggerState.Opened:
					OpenTrigger();

					break;

				case TriggerState.Closed:
					if (prevState == TriggerState.Opened)
						CloseTrigger();

					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			StateChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OpenTrigger()
		{
			if (StateTrigger.IsLoaded == false)
				return;

			if (IsClosing)
			{
				IsClosing = false;

				return;
			}

			OpenTriggerCore();
		}

		public void OpenTriggerCore()
		{
			if (DelayTrigger != null)
			{
				if (IsActuallyOpen == false)
					DelayTrigger.InvokeOpen();
				else
					DelayTrigger.RevokeClose();
			}
			else
				OpenTriggerImpl();
		}

		private void OpenTriggerImpl()
		{
			foreach (var trigger in ActualTriggers)
				trigger.IsEnabled = StateTrigger.IsEnabled;

			if (StateTrigger.IsEnabled == false)
				return;

			IsOpening = true;

			LoadActions(StateTrigger.Root);

			foreach (var action in ActualEnterActions)
				action.Invoke();

			SourceSetterGroup?.Apply();

			foreach (var setter in ActualSetters)
				setter.Apply();

			// Setter or Action caused trigger close
			if (IsOpening == false && TriggerState == TriggerState.Closed)
			{
				CloseTrigger();

				return;
			}

			IsOpening = false;
			IsActuallyOpen = true;
		}

		internal void UnloadActions(IInteractivityRoot root)
		{
			if (Status != StateTriggerStatus.InitializedActions)
				return;

			_triggers?.Unload(root);
			_enterActions?.Unload(root);
			_exitActions?.Unload(root);
			_setters?.Unload(root);
			SourceSetterGroup?.Unload(root);

			Status = StateTriggerStatus.Initialized;
		}

		public void UnloadCore(IInteractivityRoot root)
		{
			TriggerState = TriggerState.Closed;

			DeinitializeTrigger(root);
			UnloadActions(root);

			Status = StateTriggerStatus.Default;
		}

		public void UpdateTriggerState()
		{
			var isReady = Status >= StateTriggerStatus.Initialized;

			if (isReady == false && TriggerState == TriggerState.Undefined)
				return;

			TriggerState = isReady && StateTrigger.IsEnabled ? ApplyInvert(StateTrigger.UpdateState()) : TriggerState.Closed;
		}

		private enum StateTriggerStatus
		{
			Default = 0,
			Initializing = 1,
			Initialized = 2,
			InitializedActions = 3
		}

		private static class PackedDefinition
		{
			public static readonly PackedEnumItemDefinition<TriggerState> State;
			public static readonly PackedEnumItemDefinition<StateTriggerStatus> Status;
			public static readonly PackedBoolItemDefinition IsActuallyOpen;

			public static readonly PackedBoolItemDefinition IsOpening;
			public static readonly PackedBoolItemDefinition IsClosing;

			public static readonly PackedBoolItemDefinition Invert;

			static PackedDefinition()
			{
				var allocator = InteractivityObject.GetAllocator<TStateTrigger>();

				State = allocator.AllocateEnumItem<TriggerState>();
				Status = allocator.AllocateEnumItem<StateTriggerStatus>();
				IsActuallyOpen = allocator.AllocateBoolItem();

				IsOpening = allocator.AllocateBoolItem();
				IsClosing = allocator.AllocateBoolItem();

				Invert = allocator.AllocateBoolItem();
			}
		}

		private class InnerSetterCollection : SetterCollectionBase
		{
			public InnerSetterCollection(StateTriggerImplementation<TStateTrigger, TStateTriggerImplementation> implementation) : base(implementation.StateTrigger)
			{
				Implementation = implementation;
			}

			public StateTriggerImplementation<TStateTrigger, TStateTriggerImplementation> Implementation { get; }

			protected override bool IsApplied => Implementation.IsActuallyOpen;

			internal override InteractivityCollection<SetterBase> CreateInstance(IInteractivityObject parent)
			{
				return new InnerSetterCollection(((TStateTrigger)parent).Implementation);
			}
		}

		private class InnerTriggerCollection : TriggerCollectionBase
		{
			public InnerTriggerCollection(StateTriggerImplementation<TStateTrigger, TStateTriggerImplementation> implementation) : base(implementation.StateTrigger)
			{
			}

			internal override InteractivityCollection<TriggerBase> CreateInstance(IInteractivityObject parent)
			{
				return new InnerTriggerCollection(((TStateTrigger)parent).Implementation);
			}
		}
	}
}