// <copyright file="VirtualStackPanelLayoutBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
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
		private VirtualMeasureContext _context;

		protected VirtualStackPanelLayoutBase(IVirtualStackPanel panel) : base(panel)
		{
		}

		protected UIElementCollection Children => Panel.Children;

		protected abstract double DirectSmallChange { get; }

		protected abstract double DirectWheelChange { get; }

		public int FirstIndex => _context.FirstIndex;

		public int FirstVisibleIndex => _context.FirstVisibleIndex;

		protected virtual double IndirectSmallChange => ScrollViewUtils.DefaultPixelSmallChange;

		protected virtual double IndirectWheelChange => ScrollViewUtils.DefaultPixelWheelChange;

		public bool IsBringIntoViewRequested => BringIntoViewRequest != null;

		private long LastArrangeFrame { get; set; } = -1;

		public int LastIndex => _context.LastIndex;

		public int LastVisibleIndex => _context.LastVisibleIndex;

		public Orientation Orientation => Panel.Orientation;

		internal bool PreserveScrollInfo { get; set; }

		public bool ScrollInfoDirty { get; protected set; }

		public abstract ScrollUnit ScrollUnit { get; }

		protected VirtualUIElementCollectionInserter UIElementInserter { get; } = new();

		private double IndirectScrollArrangeCompensation { get; set; }

		private double CalcIndirectArrangeCompensation()
		{
			var orientation = Orientation;

			var indirectOrientedScrollInfo = ScrollInfo.Axis(orientation.Rotate());
			var desiredOriented = _context.OrientedResult;

			if (indirectOrientedScrollInfo.Offset > 0 && desiredOriented.Indirect - indirectOrientedScrollInfo.Offset < indirectOrientedScrollInfo.Viewport)
				return Math.Min(indirectOrientedScrollInfo.Offset, indirectOrientedScrollInfo.Viewport - (desiredOriented.Indirect - indirectOrientedScrollInfo.Offset));

			return 0.0;
		}

		protected override Size ArrangeCore(Size finalSize)
		{
			var orientation = Orientation;
			var offset = new OrientedPoint(orientation);
			var finalOriented = finalSize.AsOriented(orientation);

			IndirectScrollArrangeCompensation = CalcIndirectArrangeCompensation();

			finalOriented.Indirect += IndirectScrollArrangeCompensation;

			offset.Direct -= _context.PreCacheDelta - _context.PreviewFirstVisibleOffset;

			foreach (UIElement child in Children)
			{
				var childDesiredSize = child.DesiredSize;
				var childDesiredOrientedSize = child.DesiredSize.AsOriented(orientation);

				childDesiredOrientedSize.Indirect = finalOriented.Indirect;

				var rect = new Rect(offset.Point, childDesiredOrientedSize.Size);

				ArrangeChild(child, rect);

				if (child.DesiredSize.IsCloseTo(childDesiredSize) == false)
				{
					var orientedOriginal = childDesiredSize.AsOriented(orientation);
					var orientedCurrent = child.DesiredSize.AsOriented(orientation);

					if (orientedCurrent.Indirect.IsGreaterThan(orientedOriginal.Indirect) && orientedCurrent.Indirect.IsGreaterThan(finalOriented.Indirect))
						Panel.InvalidateMeasure();
				}

				offset.Direct += childDesiredOrientedSize.Direct;
			}

			LastArrangeFrame = FrameCounter.Frame;

			return finalOriented.Size;
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
			var directScrollView = new AxisScrollInfo(orientation, orientedOffset.Direct, orientedViewport.Direct, orientedExtent.Direct);
			var indirectScrollView = new AxisScrollInfo(orientation.Rotate(), orientedOffset.Indirect, orientedViewport.Indirect, orientedExtent.Indirect);

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

			return new ItemLayoutInformation(item, GetLayoutSlot(item), new Rect(_context.ViewportSize));
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
			_context = new VirtualMeasureContext(this, availableSize);

			try
			{
				UIElementInserter.Enter(Children);

				_context.Measure(Panel.FocusedIndex);
			}
			finally
			{
				UIElementInserter.Leave();
			}

			var scrollInfo = CalcScrollInfo(ref _context);

			if (PreserveScrollInfo == false)
			{
				ScrollInfo = scrollInfo;
				ScrollInfoDirty = false;

				if (_context.BringIntoViewResult)
					BringIntoViewRequest = null;
			}
			else
				ScrollInfoDirty = scrollInfo.Equals(ScrollInfo) == false;

			UpdateTransform();

			return _context.ViewportSize;
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

			if (IndirectScrollArrangeCompensation.IsCloseTo(CalcIndirectArrangeCompensation()) == false)
				Panel.InvalidateArrange();

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

		public void UpdateScrollInfo()
		{
			ScrollInfo = CalcScrollInfo(ref _context);
		}

		private protected void UpdateTransform()
		{
			var transform = Panel.Transform;
			var offset = ScrollInfo.ClampOffset(Offset);

			if (Orientation == Orientation.Vertical)
				transform.TranslateX = -offset.X.LayoutRoundX(RoundingMode.MidPointFromZero);
			else
				transform.TranslateY = -offset.Y.LayoutRoundY(RoundingMode.MidPointFromZero);
		}
	}
}