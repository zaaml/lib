// <copyright file="ToolBarItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Collections;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.Overflow;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.ToolBar
{
	public class ToolBarItemsPanel : ItemsPanel<ToolBarItem>, IFlexPanel
	{
		#region Fields

		private ToolBarItemsPresenter _itemsPresenter;

		#endregion

		#region Ctors

		public ToolBarItemsPanel()
		{
			Layout = new FlexPanelLayout(this);
		}

		#endregion

		#region Properties

		private Orientation ActualOrientation => ToolBar?.ActualOrientation ?? Orientation.Horizontal;

		internal bool HasOverflowChildren { get; private set; }

		internal ToolBarItemsPresenter ItemsPresenter
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

		private ToolBarControl ToolBar => ItemsPresenter?.ToolBar;

		#endregion

		#region  Methods

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return Layout.Arrange(finalSize);
		}

		internal override ItemHostCollection<ToolBarItem> CreateHostCollectionInternal()
		{
			return new ToolBarItemHostCollection(this);
		}

		private static bool GetIsOverflow(UIElement child)
		{
			var overflowItem = (OverflowItem<ToolBarItem>) child;
			var toolBarItem = overflowItem.Item;

			return toolBarItem.IsOverflow;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return Layout.Measure(availableSize);
		}

		internal bool ResumeOverflow()
		{
			var remeasure = false;

			foreach (var toolBarItem in ItemsInternal)
				remeasure |= toolBarItem.OverflowController.Resume();

			return remeasure;
		}

		private static void SetIsOverflow(UIElement child, bool value)
		{
			var overflowItem = (OverflowItem<ToolBarItem>) child;
			var toolBarItem = overflowItem.Item;

			toolBarItem.IsOverflow = value;
		}

		internal void SuspendOverflow()
		{
			foreach (var toolBarItem in ItemsInternal)
				toolBarItem.OverflowController.Suspend();
		}

		#endregion

		#region Interface Implementations

		#region IFlexPanel

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

		#endregion

		#region IOrientedPanel

		Orientation IOrientedPanel.Orientation => ActualOrientation;

		#endregion

		#region IPanel

		IReadOnlyList<UIElement> IPanel.Elements
		{
			get
			{
				if (Children.Count > 1 && ToolBar?.IsMeasureToMinLength == true)
					return new ReadOnlyListWrapper<UIElement>(new List<UIElement> { Children[0] });

				return ReadOnlyChildren;
			}
		}

		#endregion

		#endregion

		#region  Nested Types

		private sealed class ToolBarItemHostCollection : ItemHostCollection<ToolBarItem>
		{
			#region Ctors

			public ToolBarItemHostCollection(ToolBarItemsPanel panel)
			{
				Panel = panel;
			}

			#endregion

			#region Properties

			private ToolBarItemsPanel Panel { get; }

			#endregion

			#region  Methods

			protected override void ClearCore()
			{
				Panel.Children.Clear();
			}

			protected override void InitCore(ICollection<ToolBarItem> items)
			{
				Panel.Children.Clear();

				foreach (var toolBarItem in items)
					Panel.Children.Add(toolBarItem.OverflowController.VisibleHost);
			}

			protected override void InsertCore(int index, ToolBarItem item)
			{
				Panel.Children.Insert(index, item.OverflowController.VisibleHost);
			}

			protected override void RemoveAtCore(int index)
			{
				Panel.Children.RemoveAt(index);
			}

			#endregion
		}

		#endregion
	}
}