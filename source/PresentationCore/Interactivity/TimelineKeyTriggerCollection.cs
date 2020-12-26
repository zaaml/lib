// <copyright file="TimelineKeyTriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public class TimelineKeyTriggerCollection : InteractivityCollection<TimelineKeyFrameTriggerBase>
	{
		internal TimelineKeyTriggerCollection(TimelineTrigger timelineTrigger) : base(timelineTrigger)
		{
		}

		internal override InteractivityCollection<TimelineKeyFrameTriggerBase> CreateInstance(IInteractivityObject parent)
		{
			return new TimelineKeyTriggerCollection(Trigger);
		}

		public TimelineTrigger Trigger => (TimelineTrigger) Parent;

		protected override void OnItemAddedCore(TimelineKeyFrameTriggerBase item)
		{
			base.OnItemAddedCore(item);

			Trigger.OnItemAdded(item);
		}

		protected override void OnItemRemovedCore(TimelineKeyFrameTriggerBase item)
		{
			base.OnItemRemovedCore(item);

			Trigger.OnItemRemoved(item);
		}
	}
}