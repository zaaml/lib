// <copyright file="VirtualStackPanelLayoutBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;
using ScrollUnit = Zaaml.UI.Controls.ScrollView.ScrollUnit;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal abstract partial class VirtualStackPanelLayoutBase : VirtualPanelLayoutBase<IVirtualStackPanel>
	{
		private Size _pixelViewport;
		private double _preCacheDelta;

		protected VirtualStackPanelLayoutBase(IVirtualStackPanel panel) : base(panel)
		{
		}

		protected UIElementCollection Children => Panel.Children;

		protected abstract double DirectSmallChange { get; }

		protected abstract double DirectWheelChange { get; }

		protected virtual double IndirectSmallChange => ScrollViewUtils.DefaultPixelSmallChange;

		protected virtual double IndirectWheelChange => ScrollViewUtils.DefaultPixelWheelChange;

		public bool IsBringIntoViewRequested => BringIntoViewRequest != null;

		private long LastArrangeFrame { get; set; } = -1;

		public Orientation Orientation => Panel.Orientation;

		internal bool PreserveScrollInfo { get; set; }

		public bool ScrollInfoDirty { get; protected set; }

		public abstract ScrollUnit ScrollUnit { get; }

		protected VirtualUIElementCollectionInserter UIElementInserter { get; } = new VirtualUIElementCollectionInserter();

		protected override Size ArrangeCore(Size finalSize)
		{
			var orientation = Orientation;
			var offset = new OrientedPoint(orientation);
			var finalOriented = finalSize.AsOriented(orientation);

			offset.Direct -= _preCacheDelta;

			foreach (UIElement child in Children)
			{
				var size = child.DesiredSize.AsOriented(orientation);

				size.Indirect = finalOriented.Indirect;

				var rect = new Rect(offset.Point, size.Size);

				ArrangeChild(child, rect);

				offset.Direct += size.Direct;
			}

			LastArrangeFrame = FrameCounter.Frame;

			return finalSize;
		}

		private protected abstract int CalcFirstVisibleIndex(Vector offset, out double localFirstVisibleOffset);

		private protected abstract ScrollInfo CalcScrollInfo(ref VirtualMeasureContext context);

		public void EnqueueBringIntoView(BringIntoViewRequest request)
		{
			BringIntoViewRequest = request;
		}

		protected override void ExecuteScrollCommand(ScrollCommandKind command)
		{
			var commandOrientation = ScrollViewUtils.GetCommandOrientation(command);
			var orientation = Orientation;

			var orientedViewport = Viewport.AsOriented(orientation);
			var orientedExtent = Extent.AsOriented(orientation);
			var orientedOffset = Offset.AsOriented(orientation);
			var directScrollView = new OrientedScrollInfo(orientation, orientedOffset.Direct, orientedViewport.Direct, orientedExtent.Direct);
			var indirectScrollView = new OrientedScrollInfo(orientation.Rotate(), orientedOffset.Indirect, orientedViewport.Indirect, orientedExtent.Indirect);

			var directSmallChange = commandOrientation == orientation ? DirectSmallChange : IndirectSmallChange;
			var directWheelChange = commandOrientation == orientation ? DirectWheelChange : IndirectWheelChange;

			directScrollView.ExecuteScrollCommand(command, directSmallChange, directWheelChange);

			var indirectSmallChange = commandOrientation != orientation ? DirectSmallChange : IndirectSmallChange;
			var indirectWheelChange = commandOrientation != orientation ? DirectWheelChange : IndirectWheelChange;

			indirectScrollView.ExecuteScrollCommand(command, indirectSmallChange, indirectWheelChange);

			orientedOffset.Direct = directScrollView.Offset;
			orientedOffset.Indirect = indirectScrollView.Offset;

			Offset = orientedOffset.Vector;
		}

		internal ItemLayoutInformation GetLayoutInformation(FrameworkElement element)
		{
			return GetLayoutInformation(Source.GetIndexFromItem(element));
		}

		internal ItemLayoutInformation GetLayoutInformation(int index)
		{
			if (LastArrangeFrame == -1)
				return ItemLayoutInformation.Empty;

			if (index < 0 || index >= ItemsCount)
				return ItemLayoutInformation.Empty;

			var item = Source.GetCurrent(index) as FrameworkElement;

			if (item == null || Children.Contains(item) == false)
				return ItemLayoutInformation.Empty;

			return new ItemLayoutInformation(item, GetLayoutSlot(item), new Rect(_pixelViewport));
		}

		private Rect GetLayoutSlot(FrameworkElement item)
		{
			if (item is ILayoutInformation layoutInformation)
				return layoutInformation.ArrangeRect;

			return LayoutInformation.GetLayoutSlot(item);
		}

		private protected virtual void MeasureChild(int index, UIElement child, Size constraint, ref VirtualMeasureContext context)
		{
			child.Measure(constraint);
		}

		protected override Size MeasureCore(Size availableSize)
		{
			try
			{
				Source?.EnterGeneration();

				return MeasureImpl(availableSize);
			}
			finally
			{
				Source?.LeaveGeneration();
			}
		}

		private Size MeasureImpl(Size availableSize)
		{
			var context = new VirtualMeasureContext(this, availableSize);

			try
			{
				UIElementInserter.Enter(Children);

				context.Measure(Panel.FocusedIndex);
			}
			finally
			{
				UIElementInserter.Leave();
			}

			var scrollInfo = CalcScrollInfo(ref context);

			if (PreserveScrollInfo == false)
			{
				ScrollInfo = scrollInfo;
				ScrollInfoDirty = false;

				if (context.BringIntoViewResult)
					BringIntoViewRequest = null;
			}
			else
				ScrollInfoDirty = scrollInfo.Equals(ScrollInfo) == false;

			_pixelViewport = context.ViewportSize;
			_preCacheDelta = context.PreCacheDelta - context.PreviewFirstVisibleOffset;

			UpdateTransform();

			return _pixelViewport;
		}

		internal void OnItemAttachedInternal(UIElement element)
		{
		}

		internal void OnItemAttachingInternal(UIElement element)
		{
		}

		internal void OnItemDetachedInternal(UIElement element)
		{
		}

		internal void OnItemDetachingInternal(UIElement element)
		{
			if (UIElementInserter.Collection == null)
			{
				var index = Children.IndexOf(element);

				if (index != -1)
					Children.RemoveAt(index);
			}
			else
				UIElementInserter.Remove(element);
		}

		protected override void OnScrollInfoChanged(ScrollInfoChangedEventArgs e)
		{
			base.OnScrollInfoChanged(e);

			if (CalcFirstVisibleIndex(e.OldInfo.Offset, out var oldLocalOffset) != CalcFirstVisibleIndex(e.NewInfo.Offset, out var newLocalOffset) || oldLocalOffset.IsCloseTo(newLocalOffset) == false)
				Panel.InvalidateMeasure();

			UpdateTransform();
		}

		private protected virtual void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
		}

		internal void OnSourceCollectionChangedInternal(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			OnSourceCollectionChanged(notifyCollectionChangedEventArgs);
		}

		private UIElement Realize(int index)
		{
			return Source.Realize(index);
		}

		private protected void UpdateTransform()
		{
			var transform = Panel.Transform;
			var offset = ScrollInfo.ClampOffset(Offset);

			if (Orientation == Orientation.Vertical)
				transform.TranslateX = -offset.X;
			else
				transform.TranslateY = -offset.Y;
		}
	}
}