// <copyright file="TriggerCondition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty(nameof(Trigger))]
	public sealed class TriggerCondition : ConditionBase
	{
		private StateTriggerBase _trigger;

		internal override IEnumerable<InteractivityObject> Children
		{
			get { yield return Trigger; }
		}

		public StateTriggerBase Trigger
		{
			get => _trigger;
			set
			{
				if (ReferenceEquals(_trigger, value))
					return;

				if (_trigger != null)
				{
					_trigger.StateChanged -= TriggerOnStateChanged;

					if (ReferenceEquals(_trigger.Parent, this) == false)
						throw new InvalidOperationException();

					_trigger.Parent = null;
					_trigger.IsEnabled = false;

					if (IsLoaded)
						_trigger.Unload();
				}

				_trigger = value;

				if (_trigger != null)
				{
					if (ReferenceEquals(_trigger.Parent, null) == false)
						throw new InvalidOperationException();

					_trigger.Parent = this;
					_trigger.IsEnabled = true;

					if (IsLoaded)
						_trigger.Load();

					_trigger.StateChanged += TriggerOnStateChanged;
				}
			}
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var conditionSource = (TriggerCondition)source;

			Trigger = conditionSource.Trigger.DeepClone<StateTriggerBase>();
		}

		protected override InteractivityObject CreateInstance()
		{
			return new TriggerCondition();
		}

		internal override void Deinitialize(IInteractivityRoot root)
		{
			base.Deinitialize(root);

			Trigger?.Unload(root);
		}

		internal override void Initialize(IInteractivityRoot root)
		{
			base.Initialize(root);

			Trigger?.Load(root);
		}

		private void TriggerOnStateChanged(object sender, EventArgs e)
		{
			UpdateConditionState();
		}

		protected override bool UpdateConditionStateCore()
		{
			return Trigger is { TriggerState: TriggerState.Opened };
		}
	}
}