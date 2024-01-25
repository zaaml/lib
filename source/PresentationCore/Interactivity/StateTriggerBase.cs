// <copyright file="StateTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("Setters")]
	public abstract class StateTriggerBase : TriggerBase, IRuntimeSetterFactory, IStateTrigger<StateTriggerBase, StateTriggerBase.StateTriggerImplementation>
	{
		public event EventHandler StateChanged
		{
			add => Implementation.StateChanged += value;
			remove => Implementation.StateChanged -= value;
		}

		public event EventHandler Opened
		{
			add => Implementation.Opened += value;
			remove => Implementation.Opened -= value;
		}

		public event EventHandler Closed
		{
			add => Implementation.Closed += value;
			remove => Implementation.Closed -= value;
		}

		static StateTriggerBase()
		{
			StateTriggerImplementation.InitPackedDefinition();
		}

		internal StateTriggerBase()
		{
			Implementation = new StateTriggerImplementation(this);
		}

		private protected DelayStateTrigger ActualDelayTrigger => Implementation.ActualDelayTrigger;

		internal sealed override IEnumerable<InteractivityObject> Children => Implementation.Children;

		public Duration CloseDelay
		{
			get => Implementation.CloseDelay;
			set => Implementation.CloseDelay = value;
		}

		public TriggerActionCollection EnterActions => Implementation.EnterActions;

		public TriggerActionCollection ExitActions => Implementation.ExitActions;

		private StateTriggerImplementation Implementation { get; }

		public bool Invert
		{
			get => Implementation.Invert;
			set => Implementation.Invert = value;
		}

		protected bool IsActuallyOpen => Implementation.IsActuallyOpen;

		protected bool IsInitialized => Implementation.IsInitialized;

		public Duration OpenDelay
		{
			get => Implementation.OpenDelay;
			set => Implementation.OpenDelay = value;
		}

		public SetterCollectionBase Setters => Implementation.Setters;

		public SetterCollection SettersSource
		{
			get => Implementation.SettersSource;
			set => Implementation.SettersSource = value;
		}

		public TriggerCollectionBase Triggers => Implementation.Triggers;

		public TriggerState TriggerState => Implementation.TriggerState;

		protected virtual void CloseTriggerCore()
		{
			Implementation.CloseTriggerCore();
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceTrigger = (StateTriggerBase)source;

			Implementation.CopyMembersOverride(sourceTrigger.Implementation);
		}

		internal virtual void DeinitializeTrigger(IInteractivityRoot root)
		{
		}

		internal virtual void InitializeTrigger(IInteractivityRoot root)
		{
		}

		internal sealed override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Implementation.LoadCore(root);
		}

		protected virtual void OnClosedCore()
		{
		}

		protected override void OnIsEnabledChanged()
		{
			Implementation.OnIsEnabledChanged();
		}

		protected virtual void OnOpenedCore()
		{
		}

		protected virtual void OpenTriggerCore()
		{
			Implementation.OpenTriggerCore();
		}

		internal sealed override void UnloadCore(IInteractivityRoot root)
		{
			Implementation.UnloadCore(root);

			base.UnloadCore(root);
		}

		protected void UpdateTriggerState()
		{
			Implementation.UpdateTriggerState();
		}

		protected abstract TriggerState UpdateTriggerStateCore();

		RuntimeSetter IRuntimeSetterFactory.CreateSetter()
		{
			return Implementation.CreateSetter();
		}

		IEnumerable<InteractivityObject> IStateTrigger<StateTriggerBase, StateTriggerImplementation>.BaseChildren => base.Children;

		void IStateTrigger<StateTriggerBase, StateTriggerImplementation>.OnOpened()
		{
			OnOpenedCore();
		}

		void IStateTrigger<StateTriggerBase, StateTriggerImplementation>.OnClosed()
		{
			OnClosedCore();
		}

		TriggerState IStateTrigger<StateTriggerBase, StateTriggerImplementation>.UpdateState()
		{
			return UpdateTriggerStateCore();
		}

		void IStateTrigger<StateTriggerBase, StateTriggerImplementation>.DeinitializeTrigger(IInteractivityRoot root)
		{
			DeinitializeTrigger(root);
		}

		void IStateTrigger<StateTriggerBase, StateTriggerImplementation>.InitializeTrigger(IInteractivityRoot root)
		{
			InitializeTrigger(root);
		}

		StateTriggerImplementation IStateTrigger<StateTriggerBase, StateTriggerImplementation>.Implementation => Implementation;

		private sealed class StateTriggerImplementation : StateTriggerImplementation<StateTriggerBase, StateTriggerImplementation>
		{
			public StateTriggerImplementation(StateTriggerBase stateTrigger) : base(stateTrigger)
			{
			}
		}
	}
}