// <copyright file="PopupTriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class PopupTriggerCollection : DependencyObjectCollectionBase<PopupTrigger>
	{
		internal PopupTriggerCollection(CompositePopupTrigger compositeTrigger)
		{
			CompositeTrigger = compositeTrigger;
		}

		private CompositePopupTrigger CompositeTrigger { get; }

		protected override void OnItemAdded(PopupTrigger trigger)
		{
			base.OnItemAdded(trigger);

			trigger.ActualIsOpenChanged += OnTriggerIsOpenChanged;
		}

		protected override void OnItemRemoved(PopupTrigger trigger)
		{
			base.OnItemRemoved(trigger);

			trigger.ActualIsOpenChanged -= OnTriggerIsOpenChanged;
		}

		private void OnTriggerIsOpenChanged(object sender, EventArgs e)
		{
			CompositeTrigger.UpdateIsOpen();
		}
	}
}