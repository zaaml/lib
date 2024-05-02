// <copyright file="NavigationViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	internal sealed class NavigationViewSelectorAdvisor : CollectionSelectorAdvisorBase<NavigationViewItem>
	{
		public NavigationViewSelectorAdvisor(NavigationViewFrame navigationViewControl) : base(navigationViewControl, navigationViewControl.Items)
		{
			NavigationViewFrame = navigationViewControl;
		}

		public override bool HasSource => false;

		public override bool IsVirtualizing => false;

		public NavigationViewFrame NavigationViewFrame { get; }

		public override bool CanSelectItem(NavigationViewItem item)
		{
			return item.ActualCanSelect;
		}

		public override int GetIndexOfSource(object source)
		{
			return -1;
		}

		public override bool GetItemSelected(NavigationViewItem item)
		{
			return item.IsSelected;
		}

		public override object GetSource(int index)
		{
			return null;
		}

		public override object GetValue(NavigationViewItem item, object source)
		{
			return null;
		}

		public override object GetSource(NavigationViewItem item)
		{
			return null;
		}

		public override bool GetSourceSelected(object source)
		{
			return false;
		}

		public override void Lock(NavigationViewItem item)
		{
		}

		public override void SetItemSelected(NavigationViewItem item, bool value)
		{
			item.SetIsSelectedInternal(value);
		}

		public override void SetSourceSelected(object source, bool value)
		{
		}

		public override bool TryGetItemBySource(object source, bool ensure, out NavigationViewItem item)
		{
			item = null;

			return false;
		}

		public override bool TryGetItemByValue(object value, bool ensure, out NavigationViewItem item)
		{
			item = null;

			return false;
		}

		public override bool TryCreateSelection(int index, bool ensure, out Selection<NavigationViewItem> selection)
		{
			selection = Selection<NavigationViewItem>.Empty;

			return false;
		}

		public override bool TryCreateSelection(object source, bool ensure, out Selection<NavigationViewItem> selection)
		{
			selection = Selection<NavigationViewItem>.Empty;

			return false;
		}

		public override void Unlock(NavigationViewItem item)
		{
		}
	}
}