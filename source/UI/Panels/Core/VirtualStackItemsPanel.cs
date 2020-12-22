// <copyright file="VirtualStackItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Interfaces;
using Zaaml.UI.Panels.VirtualStackPanelLayout;
using Zaaml.UI.Utils;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Panels.Core
{
	public abstract class VirtualStackItemsPanel<TItem> : ItemsPanel<TItem>, IVirtualStackPanel, IVirtualItemsHost<TItem>
		where TItem : FrameworkElement
	{
		private IVirtualItemCollection _actualVirtualItemCollection;
		private FlickeringReducer<TItem> _flickeringReducer;
		private IVirtualItemCollection _source;

		protected VirtualStackItemsPanel()
		{
			Layout = new VirtualUnitStackPanelLayout(this);
			RenderTransform = Transform.Transform;
		}

		private IVirtualItemCollection ActualVirtualItemCollection
		{
			get => _actualVirtualItemCollection;
			set
			{
				if (ReferenceEquals(_actualVirtualItemCollection, value))
					return;

				if (_actualVirtualItemCollection != null)
					DetachVirtualCollection();

				_actualVirtualItemCollection = value;

				if (_actualVirtualItemCollection != null)
					AttachVirtualCollection();
			}
		}

		private protected virtual int LeadingTrailingLimitCore => 2;

		protected abstract int FocusedIndex { get; }

		protected override bool HasLogicalOrientation => true;

		private VirtualUnitStackPanelLayout Layout { get; }

		protected sealed override Orientation LogicalOrientation => Orientation;

		protected abstract Orientation Orientation { get; }

		internal bool ReduceMouseHoverFlickering
		{
			get => _flickeringReducer != null;
			set
			{
				if (ReduceMouseHoverFlickering == value)
					return;

				_flickeringReducer = value ? CreateFlickeringReducer() : _flickeringReducer.DisposeExchange();
			}
		}

		private IScrollViewPanelLayout ScrollViewLayout => Layout;

		private IVirtualItemCollection Source
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				_source = value;

				ActualVirtualItemCollection = Source ?? VirtualItemCollection;
			}
		}

		private CompositeTransform Transform { get; } = new CompositeTransform();

		private protected abstract IVirtualItemCollection VirtualItemCollection { get; }

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var arrangeOverride = Layout.Arrange(finalSize);

			_flickeringReducer?.OnArrange();

			return arrangeOverride;
		}

		private void AttachVirtualCollection()
		{
			ActualVirtualItemCollection.ItemHost = this;
			InvalidateMeasure();
		}

		private protected override void BringIntoView(BringIntoViewRequest<TItem> request)
		{
			Layout.BringIntoView(request);

			ScrollView?.UpdateScrollOffsetCacheInternal();
		}

		private protected abstract FlickeringReducer<TItem> CreateFlickeringReducer();

		private void DetachVirtualCollection()
		{
			ActualVirtualItemCollection.ItemHost = null;
			Children.Clear();
		}

		private protected override void EnqueueBringIntoView(BringIntoViewRequest<TItem> request)
		{
			Layout.EnqueueBringIntoView(request);
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return null;
		}

		private protected override ItemLayoutInformation GetLayoutInformation(int index)
		{
			return Layout.GetLayoutInformation(index);
		}

		private protected override ItemLayoutInformation GetLayoutInformation(TItem item)
		{
			return Layout.GetLayoutInformation(item);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			ActualVirtualItemCollection = Source ?? VirtualItemCollection;

			var layoutContext = LayoutContext.GetContext(this);

			if (layoutContext != null)
			{
				availableSize = availableSize.Clamp(new Size(0, 0), layoutContext.MaxAvailableSize);
				Layout.PreserveScrollInfo = layoutContext.MeasureContextPass == MeasureContextPass.MeasureToContent;
			}
			else
			{
				Layout.PreserveScrollInfo = false;
			}

			var measureOverrideCore = Layout.Measure(availableSize);

			return measureOverrideCore;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_flickeringReducer?.OnMouseMove(e);

			base.OnMouseMove(e);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			_flickeringReducer?.OnMouseWheel();
		}

		bool IVirtualItemsHost.IsVirtualizing => true;

		IVirtualItemCollection IVirtualItemsHost.VirtualSource
		{
			get => Source;
			set => Source = value;
		}

		void IVirtualItemsHost.OnItemAttaching(UIElement element)
		{
			Layout.OnItemAttachingInternal(element);
		}

		void IVirtualItemsHost.OnItemAttached(UIElement element)
		{
			Layout.OnItemAttachedInternal(element);
		}

		void IVirtualItemsHost.OnItemDetaching(UIElement element)
		{
			Layout.OnItemDetachingInternal(element);
		}

		void IVirtualItemsHost.OnItemDetached(UIElement element)
		{
			Layout.OnItemDetachedInternal(element);
		}

		int IVirtualStackPanel.LeadingTrailingLimit => LeadingTrailingLimitCore;

		CompositeTransform IVirtualStackPanel.Transform => Transform;

		Orientation IOrientedPanel.Orientation => Orientation;

		int IVirtualStackPanel.FocusedIndex => FocusedIndex;

		IVirtualItemCollection IVirtualPanel.VirtualSource => ActualVirtualItemCollection;

		event EventHandler<ScrollInfoChangedEventArgs> IScrollViewPanel.ScrollInfoChanged
		{
			add => ScrollViewLayout.ScrollInfoChanged += value;
			remove => ScrollViewLayout.ScrollInfoChanged -= value;
		}

		event EventHandler<OffsetChangedEventArgs> IScrollViewPanel.OffsetChanged
		{
			add => ScrollViewLayout.OffsetChanged += value;
			remove => ScrollViewLayout.OffsetChanged -= value;
		}

		bool IScrollViewPanel.CanHorizontallyScroll
		{
			get => ScrollViewLayout.CanHorizontallyScroll;
			set => ScrollViewLayout.CanHorizontallyScroll = value;
		}

		bool IScrollViewPanel.CanVerticallyScroll
		{
			get => ScrollViewLayout.CanVerticallyScroll;
			set => ScrollViewLayout.CanVerticallyScroll = value;
		}

		Size IScrollViewPanel.Extent => ScrollViewLayout.Extent;

		Vector IScrollViewPanel.Offset
		{
			get => ScrollViewLayout.Offset;
			set => ScrollViewLayout.Offset = value;
		}

		Size IScrollViewPanel.Viewport => ScrollViewLayout.Viewport;

		void IScrollViewPanel.ExecuteScrollCommand(ScrollCommandKind command)
		{
			ScrollViewLayout.ExecuteScrollCommand(command);
		}
	}
}