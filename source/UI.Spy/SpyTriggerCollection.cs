// <copyright file="SpyTriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyTriggerCollection : InheritanceContextDependencyObjectCollection<SpyTrigger>
	{
		internal SpyTriggerCollection(SpyMultiTrigger multiTrigger)
		{
			MultiTrigger = multiTrigger;
		}

		public SpyMultiTrigger MultiTrigger { get; }

		protected override void OnItemAdded(SpyTrigger trigger)
		{
			base.OnItemAdded(trigger);

			trigger.IsOpenChanged += OnTriggerIsOpenChanged;

			UpdateState();
		}

		protected override void OnItemRemoved(SpyTrigger trigger)
		{
			trigger.IsOpenChanged -= OnTriggerIsOpenChanged;

			base.OnItemRemoved(trigger);

			UpdateState();
		}

		private void OnTriggerIsOpenChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		private void UpdateState()
		{
			MultiTrigger.UpdateStateInternal();
		}
	}
}