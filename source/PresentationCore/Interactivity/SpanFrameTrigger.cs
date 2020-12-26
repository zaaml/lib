// <copyright file="SpanFrameTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty(nameof(Setters))]
	public sealed class SpanFrameTrigger : TimelineKeyFrameTriggerBase, IRuntimeSetterFactory, IStateTrigger<SpanFrameTrigger, SpanFrameTrigger.StateTriggerImplementation>
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

		static SpanFrameTrigger()
		{
			StateTriggerImplementation.InitPackedDefinition();
		}

		public SpanFrameTrigger()
		{
			Implementation = new StateTriggerImplementation(this);
		}

		internal override IEnumerable<InteractivityObject> Children => Implementation.Children;

		public Duration Duration { get; set; }

		public TriggerActionCollection EnterActions => Implementation.EnterActions;

		public TriggerActionCollection ExitActions => Implementation.ExitActions;

		private StateTriggerImplementation Implementation { get; }

		public bool Invert
		{
			get => Implementation.Invert;
			set => Implementation.Invert = value;
		}

		public SetterCollectionBase Setters => Implementation.Setters;

		public SetterCollection SettersSource
		{
			get => Implementation.SettersSource;
			set => Implementation.SettersSource = value;
		}

		public TriggerCollectionBase Triggers => Implementation.Triggers;

		public TriggerState TriggerState => Implementation.TriggerState;

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceTrigger = (SpanFrameTrigger) source;

			Implementation.CopyMembersOverride(sourceTrigger.Implementation);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new SpanFrameTrigger();
		}

		private bool IsOpen()
		{
			if (TimelineTrigger == null)
				return false;

			bool isEnabled;

			var time = TimelineTrigger.Time;

			if (Duration.HasTimeSpan)
				isEnabled = time >= ActualOffset && time <= ActualOffset + Duration.TimeSpan;
			else if (Duration == Duration.Forever)
				isEnabled = time >= ActualOffset;
			else
				isEnabled = false;

			return isEnabled;
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Implementation.LoadCore(root);
		}

		protected override void OnIsEnabledChanged()
		{
			Implementation.OnIsEnabledChanged();
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Implementation.UnloadCore(root);

			base.UnloadCore(root);
		}

		protected override void UpdateStateCore()
		{
			Implementation.UpdateTriggerState();
		}

		RuntimeSetter IRuntimeSetterFactory.CreateSetter()
		{
			return Implementation.CreateSetter();
		}

		IEnumerable<InteractivityObject> IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.BaseChildren => base.Children;

		void IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.OnOpened()
		{
		}

		void IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.OnClosed()
		{
		}

		TriggerState IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.UpdateState()
		{
			return IsOpen() ? TriggerState.Opened : TriggerState.Closed;
		}

		void IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.DeinitializeTrigger(IInteractivityRoot root)
		{
		}

		void IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.InitializeTrigger(IInteractivityRoot root)
		{
		}

		StateTriggerImplementation IStateTrigger<SpanFrameTrigger, StateTriggerImplementation>.Implementation => Implementation;

		private sealed class StateTriggerImplementation : StateTriggerImplementation<SpanFrameTrigger, StateTriggerImplementation>
		{
			public StateTriggerImplementation(SpanFrameTrigger stateTrigger) : base(stateTrigger)
			{
			}
		}
	}
}