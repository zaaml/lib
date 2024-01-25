// <copyright file="TrackBarItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
	public sealed class TrackBarItemCollection : DependencyObjectCollectionBase<TrackBarItem>
	{
		internal TrackBarItemCollection(TrackBarControl trackBar)
		{
			TrackBar = trackBar;
		}

		internal TrackBarControl TrackBar { get; }

		protected override void OnItemAdded(TrackBarItem item)
		{
			base.OnItemAdded(item);

			TrackBar.OnItemAddedInternal(item);
		}

		protected override void OnItemRemoved(TrackBarItem item)
		{
			TrackBar.OnItemRemovedInternal(item);

			base.OnItemRemoved(item);
		}
	}
}