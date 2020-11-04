// <copyright file="StateTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("Setters")]
	public abstract class StateTriggerBase : TriggerBase, IRuntimeSetterFactory
	{
    //TODO implement Invert property for simple != conditions (also for conditions, condition groups, VisualStateTrigger, ...)

		#region Fields

		private TriggerActionCollection _enterActions;
		private TriggerActionCollection _exitActions;
		private TriggerInnerSetterCollection _setters;
		private TriggerInnerTriggerCollection _triggers;

		public event EventHandler StateChanged;
		public event EventHandler Opened;
		public event EventHandler Closed;

		#endregion

		#region Ctors

		private static readonly uint DefaultPackedValue;

		static StateTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.State.SetValue(ref DefaultPackedValue, TriggerState.Undefined);
			PackedDefinition.Status.SetValue(ref DefaultPackedValue, StateTriggerStatus.Default);
		}

		internal StateTriggerBase()
		{
			PackedValue |= DefaultPackedValue;
		}

    #endregion

    #region Properties

	  internal sealed override IEnumerable<InteractivityObject> Children => base.Children.Concat(ActualEnterActions).Concat(ActualSetters).Concat(ActualExitActions).Concat(ActualTriggers);

    private IEnumerable<TriggerActionBase> ActualEnterActions => _enterActions ?? Enumerable.Empty<TriggerActionBase>();

		private IEnumerable<TriggerActionBase> ActualExitActions => _exitActions ?? Enumerable.Empty<TriggerActionBase>();

		private IEnumerable<SetterBase> ActualSetters => _setters ?? Enumerable.Empty<SetterBase>();

		private IEnumerable<TriggerBase> ActualTriggers => _triggers ?? Enumerable.Empty<TriggerBase>();

		public TriggerActionCollection EnterActions => _enterActions ?? (_enterActions = new TriggerActionCollection(this));

		public TriggerActionCollection ExitActions => _exitActions ?? (_exitActions = new TriggerActionCollection(this));

	  private SetterGroup SourceSetterGroup
	  {
	    get => MutableData.GetSetValueOrDefault<SetterGroup>();
	    set => MutableData = value;
	  }

	  private SetterGroup EnsureSourceSetterGroup
	  {
	    get
	    {
	      var setterGroup = SourceSetterGroup;

        if (setterGroup != null)
	        return setterGroup;

        setterGroup = new SetterGroup { Parent = this };
        SourceSetterGroup = setterGroup;

        if (IsLoaded)
          setterGroup.Load();

        if (IsActuallyOpen)
          setterGroup.Apply();

        return setterGroup;
	    }
	  }

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

    protected bool IsActuallyOpen
		{
			get => PackedDefinition.IsActuallyOpen.GetValue(PackedValue);
			private set
			{
				if (IsActuallyOpen == value)
					return;

				PackedDefinition.IsActuallyOpen.SetValue(ref PackedValue, value);

				if (value)
					OnOpened();
				else
					OnClosed();
			}
		}

		private bool IsClosing
		{
			get => PackedDefinition.IsClosing.GetValue(PackedValue);
			set => PackedDefinition.IsClosing.SetValue(ref PackedValue, value);
		}

    public bool Invert
    {
      get => PackedDefinition.Invert.GetValue(PackedValue);
      set
      {
        if (Invert == value)
          return;

        PackedDefinition.Invert.SetValue(ref PackedValue, value);

        UpdateTriggerState();
      }
    }

    protected bool IsInitialized => Status >= StateTriggerStatus.Initialized;

		private bool IsOpening
		{
			get => PackedDefinition.IsOpening.GetValue(PackedValue);
			set => PackedDefinition.IsOpening.SetValue(ref PackedValue, value);
		}

		public SetterCollectionBase Setters => _setters ?? (_setters = new TriggerInnerSetterCollection(this));

		private StateTriggerStatus Status
		{
			get => PackedDefinition.Status.GetValue(PackedValue);
			set => PackedDefinition.Status.SetValue(ref PackedValue, value);
		}

		public TriggerCollectionBase Triggers => _triggers ?? (_triggers = new TriggerInnerTriggerCollection(this));

		public TriggerState TriggerState
		{
			get => PackedDefinition.State.GetValue(PackedValue);
			private set
			{
				var state = TriggerState;

				if (state == value)
					return;

				var prevState = state;
				PackedDefinition.State.SetValue(ref PackedValue, value);
				OnStateChanged(prevState);
			}
		}

		#endregion

		#region  Methods

		private void CloseTrigger()
		{
			if (IsOpening)
			{
				IsOpening = false;
				return;
			}

			CloseTriggerCore();
		}

		protected virtual void CloseTriggerCore()
		{
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

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			var sourceTrigger = (DelayStateTriggerBase) source;

			_triggers = sourceTrigger._triggers?.DeepCloneCollection<TriggerInnerTriggerCollection, TriggerBase>(this);
			_setters = sourceTrigger._setters?.DeepCloneCollection<TriggerInnerSetterCollection, SetterBase>(this);
			_enterActions = sourceTrigger._enterActions?.DeepCloneCollection<TriggerActionCollection, TriggerActionBase>(this);
			_exitActions = sourceTrigger._exitActions?.DeepCloneCollection<TriggerActionCollection, TriggerActionBase>(this);

		  var settersSource = sourceTrigger.SettersSource;

		  if (settersSource != null)
		    SettersSource = settersSource;

		  PackedDefinition.Invert.SetValue(ref PackedValue, sourceTrigger.Invert);
		}

		internal virtual void DeinitializeTrigger(IInteractivityRoot root)
		{
		}

		internal virtual void InitializeTrigger(IInteractivityRoot root)
		{
		}

		internal virtual void LoadActions(IInteractivityRoot root)
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

		internal sealed override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

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

		protected virtual void OnClosedCore()
		{
		}

		protected override void OnIsEnabledChanged()
		{
			UpdateTriggerState();

			if (TriggerState == TriggerState.Opened)
				foreach (var trigger in ActualTriggers)
					trigger.IsEnabled = IsEnabled;
			else
				foreach (var trigger in ActualTriggers)
					trigger.IsEnabled = false;
		}

		private void OnOpened()
		{
			OnOpenedCore();
			Opened?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnOpenedCore()
		{
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
			if (IsLoaded == false)
				return;

			if (IsClosing)
			{
				IsClosing = false;
				return;
			}

			OpenTriggerCore();
		}

		protected virtual void OpenTriggerCore()
		{
			OpenTriggerImpl();
		}

		private void OpenTriggerImpl()
		{
			foreach (var trigger in ActualTriggers)
				trigger.IsEnabled = IsEnabled;

			if (IsEnabled == false) return;

			IsOpening = true;

			LoadActions(Root);

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

		internal virtual void UnloadActions(IInteractivityRoot root)
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

		internal sealed override void UnloadCore(IInteractivityRoot root)
		{
			TriggerState = TriggerState.Closed;

			DeinitializeTrigger(root);

			UnloadActions(root);

			Status = StateTriggerStatus.Default;

			base.UnloadCore(root);
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

    protected void UpdateTriggerState()
		{
			var isReady = Status >= StateTriggerStatus.Initialized;
			if (isReady == false && TriggerState == TriggerState.Undefined)
				return;

			TriggerState = isReady && IsEnabled ? ApplyInvert(UpdateTriggerStateCore()) : TriggerState.Closed;
		}

		protected abstract TriggerState UpdateTriggerStateCore();

		#endregion

		#region Interface Implementations

		#region IRuntimeSetterFactory

		RuntimeSetter IRuntimeSetterFactory.CreateSetter()
		{
			return new TriggerRuntimeSetter();
		}

		#endregion

		#endregion

		#region  Nested Types

		private enum StateTriggerStatus
		{
			Default = 0,
			Initializing = 1,
			Initialized = 2,
			InitializedActions = 3
		}

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedEnumItemDefinition<TriggerState> State;
			public static readonly PackedEnumItemDefinition<StateTriggerStatus> Status;
			public static readonly PackedBoolItemDefinition IsActuallyOpen;

			public static readonly PackedBoolItemDefinition IsOpening;
			public static readonly PackedBoolItemDefinition IsClosing;

			public static readonly PackedBoolItemDefinition Invert;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<StateTriggerBase>();

				State = allocator.AllocateEnumItem<TriggerState>();
				Status = allocator.AllocateEnumItem<StateTriggerStatus>();
				IsActuallyOpen = allocator.AllocateBoolItem();

				IsOpening = allocator.AllocateBoolItem();
				IsClosing = allocator.AllocateBoolItem();

        Invert = allocator.AllocateBoolItem();
			}

			#endregion
		}

		private class TriggerInnerSetterCollection : SetterCollectionBase
		{
			#region Ctors

			public TriggerInnerSetterCollection(StateTriggerBase parent) : base(parent)
			{
			}

			#endregion

			#region Properties

			protected override bool IsApplied => Trigger.IsActuallyOpen;

			private DelayStateTriggerBase Trigger => (DelayStateTriggerBase) Parent;

			#endregion

			#region  Methods

			internal override InteractivityCollection<SetterBase> CreateInstance(IInteractivityObject parent)
			{
				return new TriggerInnerSetterCollection((DelayStateTriggerBase) parent);
			}

			#endregion
		}

		private class TriggerInnerTriggerCollection : TriggerCollectionBase
		{
			#region Ctors

			public TriggerInnerTriggerCollection(StateTriggerBase parent) : base(parent)
			{
			}

			#endregion

			#region  Methods

			internal override InteractivityCollection<TriggerBase> CreateInstance(IInteractivityObject parent)
			{
				return new TriggerInnerTriggerCollection((StateTriggerBase) parent);
			}

			#endregion
		}

		#endregion
	}
}