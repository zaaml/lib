// <copyright file="TabViewItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.TabView
{
	public sealed class TabViewItemsPanel : FlexItemsPanelBase<TabViewItem>, IFlexPanel
	{
		private readonly OrderedChildrenCollection _orderedChildren;

		public TabViewItemsPanel()
		{
			_orderedChildren = new OrderedChildrenCollection(this);
		}

		internal TabViewItemsPresenter ItemsPresenter { get; set; }

		internal IReadOnlyList<TabViewItem> OrderedChildren => _orderedChildren;

		private protected override FlexPanelLayout CreateLayout()
		{
			return new FlexPanelLayoutAdvanced(this);
		}

		protected override FlexElement GetFlexElement(UIElement child)
		{
			var overflowBehavior = FlexOverflowBehavior.None;
			var tabViewItem = (TabViewItem)child;

			if (_orderedChildren.Count > 0)
				overflowBehavior = tabViewItem.IsPinned || tabViewItem.IsSelected || ReferenceEquals(_orderedChildren[0], child) ? FlexOverflowBehavior.Pin | FlexOverflowBehavior.Stretch : FlexOverflowBehavior.Hide;

			return base.GetFlexElement(child).WithOverflowBehavior(overflowBehavior);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			RefreshItems();

			var size = base.MeasureOverrideCore(availableSize);
			var selectedItem = ItemsPresenter?.TabViewControl?.SelectedTabViewItem;

			if (selectedItem == null || GetIsHidden(selectedItem) == false)
				return size;

			var index = 1;

			foreach (var tabViewItem in OrderedChildren)
				tabViewItem.DisplayIndex = index++;

			selectedItem.DisplayIndex = 0;

			RefreshItems();

			size = base.MeasureOverrideCore(availableSize);

			return size;
		}

		internal void OnDisplayIndexChanged(TabViewItem tabViewItem)
		{
			InvalidateMeasure();
		}

		private void RefreshItems()
		{
			_orderedChildren.Refresh();
		}

		IReadOnlyList<UIElement> IPanel.Elements => OrderedChildren;

		private sealed class FlexPanelLayoutAdvanced : FlexPanelLayout
		{
			private readonly TabViewItemsPanel _panel;

			public FlexPanelLayoutAdvanced(TabViewItemsPanel panel) : base(panel)
			{
				_panel = panel;
			}

			private protected override bool? ShouldFillIndirect
			{
				get
				{
					var stretchDirection = _panel.FlexDefinition?.StretchDirection;

					if (stretchDirection == null)
						return null;

					return stretchDirection is FlexStretchDirection.Both or FlexStretchDirection.Expand;
				}
			}
		}

		private class OrderedChildrenCollection : IReadOnlyList<TabViewItem>
		{
			private readonly TabViewItemsPanel _panel;
			private readonly List<TabViewItem> _sortedElements = new();

			public OrderedChildrenCollection(TabViewItemsPanel panel)
			{
				_panel = panel;
			}

			public void Refresh()
			{
				var index = -1;

				foreach (TabViewItem tabViewItem in _panel.Children)
				{
					if (tabViewItem.DisplayIndex != -1)
						index = tabViewItem.DisplayIndex;
					else if (index != -1)
						tabViewItem.DisplayIndex = index;
				}

				_sortedElements.Clear();
				_sortedElements.AddRange(_panel.Children.Cast<TabViewItem>().OrderBy(t => t.IsPinned ? 0 : 1).ThenBy(t => t.DisplayIndex));
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IEnumerator<TabViewItem> GetEnumerator()
			{
				return _sortedElements.GetEnumerator();
			}

			public int Count => _sortedElements.Count;

			public TabViewItem this[int index] => _sortedElements[index];
		}
	}
}