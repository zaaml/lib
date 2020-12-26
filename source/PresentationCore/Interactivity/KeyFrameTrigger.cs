// <copyright file="KeyFrameTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("Actions")]
	public sealed class KeyFrameTrigger : TimelineKeyFrameTriggerBase
	{
		private TriggerActionCollection _actions;

		public TriggerActionCollection Actions => _actions ??= new TriggerActionCollection(this);

		private IEnumerable<TriggerActionBase> ActualActions => _actions ?? Enumerable.Empty<TriggerActionBase>();

		private long Iteration { get; set; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceTrigger = (KeyFrameTrigger) source;

			_actions = sourceTrigger._actions?.DeepCloneCollection<TriggerActionCollection, TriggerActionBase>(this);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new KeyFrameTrigger();
		}

		private void Invoke()
		{
			if (IsEnabled == false)
				return;

			foreach (var action in ActualActions)
				action.Invoke();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			foreach (var action in ActualActions)
				action.Load(root);

			base.LoadCore(root);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			foreach (var action in ActualActions)
				action.Unload(root);

			base.UnloadCore(root);
		}

		protected override void UpdateStateCore()
		{
			if (TimelineTrigger == null)
				return;

			var iteration = TimelineTrigger.Iteration;

			if (Iteration == iteration)
				return;

			var time = TimelineTrigger.Time;

			if (TimelineTrigger.IsReversed)
			{
				if (time <= ActualOffset)
				{
					Iteration = iteration;

					Invoke();
				}
			}
			else
			{
				if (time >= ActualOffset)
				{
					Iteration = iteration;

					Invoke();
				}
			}
		}
	}
}