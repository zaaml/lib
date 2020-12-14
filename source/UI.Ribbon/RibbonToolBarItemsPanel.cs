// <copyright file="RibbonToolBarItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.Overflow;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.Ribbon
{
	public class RibbonToolBarItemsPanel : ItemsPanel<RibbonItem>, IFlexPanel
	{
		private RibbonToolBarItemsPresenter _itemsPresenter;

		public RibbonToolBarItemsPanel()
		{
			Layout = new FlexPanelLayout(this);
		}

		internal bool HasOverflowChildren { get; private set; }

		internal RibbonToolBarItemsPresenter ItemsPresenter
		{
			get => _itemsPresenter;
			set
			{
				if (ReferenceEquals(_itemsPresenter, value))
					return;

				_itemsPresenter = value;

				InvalidateMeasure();
			}
		}

		private FlexPanelLayout Layout { get; }

		[UsedImplicitly] private RibbonToolBar ToolBar => ItemsPresenter?.ToolBar;

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return Layout.Arrange(finalSize);
		}

		internal override ItemHostCollection<RibbonItem> CreateHostCollectionInternal()
		{
			return new RibbonToolBarItemHostCollection(this);
		}

		private static bool GetIsOverflow(UIElement child)
		{
			var overflowItem = (OverflowItem<RibbonItem>) child;
			var ribbonItem = overflowItem.Item;

			return ribbonItem.IsOverflow;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return Layout.Measure(availableSize);
		}

		internal bool ResumeOverflow()
		{
			var remeasure = false;

			foreach (var ribbonItem in ItemsInternal)
				remeasure |= ribbonItem.OverflowController.Resume();

			return remeasure;
		}

		private static void SetIsOverflow(UIElement child, bool value)
		{
			var overflowItem = (OverflowItem<RibbonItem>) child;
			var ribbonItem = overflowItem.Item;

			ribbonItem.IsOverflow = value;
		}

		internal void SuspendOverflow()
		{
			foreach (var ribbonItem in ItemsInternal)
				ribbonItem.OverflowController.Suspend();
		}

		IFlexDistributor IFlexPanel.Distributor => FlexDistributor.LastToFirst;

		bool IFlexPanel.HasHiddenChildren
		{
			get => HasOverflowChildren;
			set => HasOverflowChildren = value;
		}

		double IFlexPanel.Spacing => 0;

		FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

		FlexElement IFlexPanel.GetFlexElement(UIElement child)
		{
			var overflowBehavior = Children.Count > 0 && ReferenceEquals(child, Children[0]) ? FlexOverflowBehavior.Pin : FlexOverflowBehavior.Hide;

			return child.GetFlexElement(this).WithStretchDirection(FlexStretchDirection.None).WithOverflowBehavior(overflowBehavior);
		}

		bool IFlexPanel.GetIsHidden(UIElement child) => GetIsOverflow(child);

		void IFlexPanel.SetIsHidden(UIElement child, bool value) => SetIsOverflow(child, value);

		Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

		private sealed class RibbonToolBarItemHostCollection : PanelHostCollectionBase<RibbonItem, RibbonToolBarItemsPanel>
		{
			public RibbonToolBarItemHostCollection(RibbonToolBarItemsPanel panel) : base(panel)
			{
			}

			protected override UIElement GetActualElement(RibbonItem item)
			{
				return item.OverflowController.VisibleHost;
			}
		}
	}
}