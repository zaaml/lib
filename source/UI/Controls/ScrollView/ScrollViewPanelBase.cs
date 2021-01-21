// <copyright file="ScrollViewPanelBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Packed;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.ScrollView
{
	public abstract class ScrollViewPanelBase : Panel, IScrollViewPanel
	{
		private Size _actualViewport;
		private FrameworkElement _child;
		private byte _packedValue;
		private Thickness _padding;
		private double _scaleX = 1.0;
		private double _scaleY = 1.0;
		private IScrollViewPanel _scrollClient;
		private ScrollInfo _scrollInfo;
		public event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged;

		internal ScrollViewPanelBase()
		{
			Background = XamlConstants.TransparentBrush;

			RenderTransformOrigin = new Point(0, 0);
			RenderTransform = Transform.Transform;
		}

		private Size ActualChildSize => Child?.DesiredSize ?? new Size();

		internal Size ActualViewport
		{
			get => _actualViewport;
			set
			{
				if (_actualViewport.IsCloseTo(value))
					return;

				_actualViewport = value;

				InvalidateMeasure();
			}
		}

		private bool CanHorizontallyScroll
		{
			get => IsScrollClient ? PackedDefinition.CanHorizontallyScroll.GetValue(_packedValue) : ScrollClient.CanHorizontallyScroll;
			set
			{
				if (CanHorizontallyScroll == value)
					return;

				if (IsScrollClient)
					PackedDefinition.CanHorizontallyScroll.SetValue(ref _packedValue, value);
				else
					ScrollClient.CanHorizontallyScroll = value;

				InvalidateMeasure();
			}
		}

		private bool CanVerticallyScroll
		{
			get => IsScrollClient ? PackedDefinition.CanVerticallyScroll.GetValue(_packedValue) : ScrollClient.CanVerticallyScroll;
			set
			{
				if (CanVerticallyScroll == value)
					return;

				if (IsScrollClient)
					PackedDefinition.CanVerticallyScroll.SetValue(ref _packedValue, value);
				else
					ScrollClient.CanVerticallyScroll = value;

				InvalidateMeasure();
			}
		}

		public FrameworkElement Child
		{
			get => _child;
			set
			{
				if (ReferenceEquals(_child, value))
					return;

				var oldChild = _child;

				Children.Clear();

				_child = value;

				UpdateScrollClient();

				if (_child != null)
					Children.Add(_child);

				ChildMeasured = false;

				OnChildChanged(oldChild, _child);
			}
		}

		private bool ChildMeasured
		{
			get => PackedDefinition.ChildMeasured.GetValue(_packedValue);
			set => PackedDefinition.ChildMeasured.SetValue(ref _packedValue, value);
		}

		private Size Extent => IsScrollClient ? ScrollInfo.Extent : ScrollClient.Extent;

		protected virtual HorizontalAlignment HorizontalContentAlignment => ScrollView?.HorizontalContentAlignment ?? HorizontalAlignment.Center;

		private bool IsScrollClient => ScrollClient == null;

		internal bool IsScrollClientInternal => IsScrollClient;

		private Vector Offset
		{
			get => IsScrollClient ? _scrollInfo.Offset : ScrollClient.Offset;
			set
			{
				var oldOffset = Offset;

				if (oldOffset.Equals(value))
					return;

				if (IsScrollClient)
					ScrollInfo = ScrollInfo.WithOffset(value);
				else
					ScrollClient.Offset = value;
			}
		}

		private Thickness Padding
		{
			get => _padding;
			set
			{
				if (_padding.Equals(value))
					return;

				_padding = value;

				UpdateScrollInfo();

				InvalidatePresenterArrange();
			}
		}

		internal Thickness PaddingInternal => Padding;

		private static RoundingMode RoundingMode => RoundingMode.MidPointFromZero;

		private double ScaleX
		{
			get => _scaleX;
			set
			{
				if (DoubleUtils.AreClose(_scaleX, value))
					return;

				_scaleX = value;

				OnScaleChanged();
			}
		}

		internal double ScaleXInternal
		{
			get => ScaleX;
			set => ScaleX = value;
		}

		private double ScaleY
		{
			get => _scaleY;
			set
			{
				if (DoubleUtils.AreClose(_scaleY, value))
					return;

				_scaleY = value;

				OnScaleChanged();
			}
		}

		internal double ScaleYInternal
		{
			get => ScaleY;
			set => ScaleY = value;
		}

		private IScrollViewPanel ScrollClient
		{
			get => _scrollClient;
			set
			{
				if (ReferenceEquals(_scrollClient, value))
					return;

				if (_scrollClient != null) 
					_scrollClient.ScrollInfoChanged -= OnScrollClientScrollInfoChanged;

				_scrollClient = value;

				if (_scrollClient != null)
				{
					_scrollClient.CanHorizontallyScroll = CanHorizontallyScroll;
					_scrollClient.CanVerticallyScroll = CanVerticallyScroll;
					_scrollClient.Offset = Offset;

					_scrollClient.ScrollInfoChanged += OnScrollClientScrollInfoChanged;
				}
			}
		}

		private ScrollInfo ScrollInfo
		{
			get => IsScrollClient ? _scrollInfo : new ScrollInfo(ScrollClient.Offset, ScrollClient.Viewport, ScrollClient.Extent);
			set
			{
				if (!IsScrollClient)
					return;

				var oldScrollInfo = ScrollInfo;

				if (oldScrollInfo.Equals(value))
					return;

				_scrollInfo = value;

				OnScrollInfoChanged(new ScrollInfoChangedEventArgs(oldScrollInfo, _scrollInfo));
			}
		}

		protected abstract ScrollViewControlBase ScrollView { get; }

		private FrameworkElement ScrollViewPresenter => (FrameworkElement) this.GetVisualParent();

		private CompositeTransform Transform { get; } = new CompositeTransform();

		protected virtual VerticalAlignment VerticalContentAlignment => ScrollView?.VerticalContentAlignment ?? VerticalAlignment.Center;

		private Size Viewport => IsScrollClient ? ScrollInfo.Viewport : ScrollClient.Viewport;

		protected sealed override Size ArrangeOverrideCore(Size finalSize)
		{
			var child = Child;

			if (child == null)
				return finalSize;

			var finalRect = new Rect(new Point(), finalSize);

			if (IsScrollClient)
			{
				var desiredRect = new Rect
				{
					Width = child.DesiredSize.Width,
					Height = child.DesiredSize.Height
				};

				var rect = RectUtils.CalcAlignBox(finalRect, desiredRect, HorizontalContentAlignment, VerticalContentAlignment);

				rect.X = DoubleUtils.Clamp(rect.X, 0, double.MaxValue);
				rect.Y = DoubleUtils.Clamp(rect.Y, 0, double.MaxValue);

				child.Arrange(rect);
			}
			else
				child.Arrange(finalRect);

			return finalSize;
		}

		internal static ElementBounds CalcElementBounds(double offsetX, double offsetY, double scaleX, double scaleY, Size childSize, Size actualViewport, Thickness padding)
		{
			var elementBounds = new ElementBounds
			{
				Bounds = new Rect(new Point(), childSize),
				ScaleX = scaleX,
				ScaleY = scaleY
			};

			if (childSize.Width * scaleX < actualViewport.Width)
			{
				elementBounds.X = (actualViewport.Width - childSize.Width) / 2;
				elementBounds.RelativeCenterX = 0.5;
				elementBounds.TranslateX = padding.Width() / 2 - offsetX;
			}
			else
			{
				elementBounds.RelativeCenterX = 0;
				elementBounds.TranslateX = -offsetX + padding.Left;
			}

			if (childSize.Height * scaleY < actualViewport.Height)
			{
				elementBounds.Y = (actualViewport.Height - childSize.Height) / 2;
				elementBounds.RelativeCenterY = 0.5;
				elementBounds.TranslateY = padding.Height() / 2 - offsetY;
			}
			else
			{
				elementBounds.RelativeCenterY = 0;
				elementBounds.TranslateY = -offsetY + padding.Top;
			}

			return elementBounds;
		}

		private Size CalcFinalMeasureSize(Size desiredSize, Size availableSize)
		{
			var horizontalResult = desiredSize.Width;
			var verticalResult = desiredSize.Height;

			return new Size(horizontalResult, verticalResult);
		}

		private void CalculatePaddingAndOffset(ScrollInfo scrollInfo, ref Vector offset, ref Thickness padding)
		{
			var scrollableSize = scrollInfo.ScrollableSize;

			if (offset.X > 0 && offset.X < scrollableSize.Width)
			{
				if (padding.Width() > 0)
				{
					var deltaLeft = Math.Min(offset.X, padding.Left);
					var deltaRight = Math.Min(scrollableSize.Width - offset.X, padding.Right);
					var delta = Math.Min(deltaLeft, deltaRight);

					padding.Left -= delta;
					padding.Right -= delta;

					offset.X -= delta;
				}
			}
			else if (offset.X > scrollableSize.Width)
			{
				var delta = offset.X - scrollableSize.Width;

				padding = padding.GetExtended(new Thickness(delta, 0, delta, 0));

				offset.X += delta;
			}
			else if (offset.X < 0)
			{
				var delta = -offset.X;

				padding = padding.GetExtended(new Thickness(delta, 0, delta, 0));

				offset.X = 0;
			}

			if (offset.Y > 0 && offset.Y < scrollableSize.Height)
			{
				if (padding.Height() > 0)
				{
					var deltaTop = Math.Min(offset.Y, padding.Top);
					var deltaBottom = Math.Min(scrollableSize.Height - offset.Y, padding.Bottom);
					var delta = Math.Min(deltaTop, deltaBottom);

					padding.Top -= delta;
					padding.Bottom -= delta;

					offset.Y -= delta;
				}
			}
			else if (offset.Y > scrollableSize.Height)
			{
				var delta = offset.Y - scrollableSize.Height;

				padding = padding.GetExtended(new Thickness(0, delta, 0, delta));

				offset.Y += delta;
			}
			else if (offset.Y < 0)
			{
				var delta = -offset.Y;

				padding = padding.GetExtended(new Thickness(0, delta, 0, delta));

				offset.Y = 0;
			}
		}

		private ScrollInfo CalculateScrollInfo(Size desiredSize, Size availableSize, double scaleX, double scaleY, Thickness padding)
		{
			var extent = desiredSize;

			extent.Width *= scaleX;
			extent.Height *= scaleY;

			if (extent.Width < availableSize.Width)
				extent.Width = availableSize.Width;

			if (extent.Height < availableSize.Height)
				extent.Height = availableSize.Height;

			extent.Width += padding.Width();
			extent.Height += padding.Height();

			return new ScrollInfo(Offset, availableSize, extent);
		}

		private ScrollInfo CalculateScrollInfo()
		{
			return CalculateScrollInfo(ActualChildSize, ActualViewport, ScaleX, ScaleY, Padding);
		}

		internal Vector CoerceRoundOffset(Vector offset)
		{
			var point = CoerceRoundOffset(new Point(offset.X, offset.Y));

			return new Vector(point.X, point.Y);
		}

		internal Point CoerceRoundOffset(Point offset)
		{
			var bounds = GetChildBounds(offset.X, offset.Y, ScaleX, ScaleY);
			var roundBounds = bounds;
			var roundLocation = bounds.TransformedLocation.LayoutRound(RoundingMode);

			roundBounds.RecalculateTranslate(roundLocation.X, roundLocation.Y);

			return new Point(offset.X + bounds.TranslateX - roundBounds.TranslateX, offset.Y + bounds.TranslateY - roundBounds.TranslateY);
		}

		public virtual void ExecuteScrollCommand(ScrollCommandKind command)
		{
			if (IsScrollClient == false)
			{
				ScrollClient.ExecuteScrollCommand(command);

				return;
			}

			Offset = ExecuteScrollCommand(ScrollInfo.ClampOffset(Offset), command);

			UpdatePaddingAndOffset(Offset);
		}

		private Vector ExecuteScrollCommand(Vector offset, ScrollCommandKind command)
		{
			return ScrollViewUtils.ExecuteScrollCommand(offset, command, Viewport, Extent, ScrollViewUtils.DefaultPixelSmallChange, ScrollViewUtils.DefaultPixelWheelChange);
		}

		private ElementBounds GetChildBounds()
		{
			var offset = ScrollInfo.ClampOffset(Offset);

			return GetChildBounds(offset.X, offset.Y, ScaleX, ScaleY);
		}

		private ElementBounds GetChildBounds(double offsetX, double offsetY, double scaleX, double scaleY)
		{
			var childSize = ActualChildSize;
			var viewport = ActualViewport;
			var padding = Padding;

			return CalcElementBounds(offsetX, offsetY, scaleX, scaleY, childSize, viewport, padding);
		}

		internal ElementBounds GetChildBoundsInternal(bool round)
		{
			return round ? GetChildBoundsRound() : GetChildBounds();
		}

		private ElementBounds GetChildBoundsRound()
		{
			var offset = CoerceRoundOffset(ScrollInfo.ClampOffset(Offset));

			return GetChildBounds(offset.X, offset.Y, ScaleX, ScaleY);
		}

		private void InvalidatePresenterArrange()
		{
			ScrollViewPresenter?.InvalidateArrange();
		}

		//internal void InvalidateScrollMeasure()
		//{
		//	InvalidateMeasure();

		//	if (ScrollClient is FrameworkElement scrollViewPanel)
		//		scrollViewPanel.InvalidateMeasure();
		//}

		protected sealed override Size MeasureOverrideCore(Size availableSize)
		{
			var child = Child;

			if (child == null)
				return new Size();

			if (ChildMeasured == false)
			{
				// Ensure template
				if (child.ApplyTemplate() == false)
					child.Measure(new Size(0, 0));

				ChildMeasured = true;
			}

			var actualViewport = ActualViewport;
			var scrollViewer = ScrollView;

			if (scrollViewer == null)
			{
				child.Measure(XamlConstants.InfiniteSize);

				return CalcFinalMeasureSize(child.DesiredSize, actualViewport);
			}

			UpdateScrollClient();
			scrollViewer.UpdateScrollViewPanelInternal();

			if (IsScrollClient == false)
			{
				child.Measure(actualViewport);

				return child.DesiredSize;
			}

			var horizontalSize = CanHorizontallyScroll ? double.PositiveInfinity : actualViewport.Width;
			var verticalSize = CanVerticallyScroll ? double.PositiveInfinity : actualViewport.Height;
			var constraint = new Size(horizontalSize, verticalSize);

			child.Measure(constraint);

			UpdateScrollInfo();

			return CalcFinalMeasureSize(child.DesiredSize, actualViewport);
		}

		protected virtual void OnChildChanged(FrameworkElement oldChild, FrameworkElement newChild)
		{
		}

		internal void OnMouseZoom(Point viewportPoint, double newZoom)
		{
			Zoom(viewportPoint, newZoom);
		}

		internal Point GetViewportPoint(Point localPoint)
		{
			return Transform.Transform.Transform(localPoint);
		}

		internal void OnMouseZoom(MouseEventArgs mouseEventArgs, double newZoom)
		{
			var mousePoint = mouseEventArgs.GetPosition(this);
			var viewportPoint = Transform.Transform.Transform(mousePoint);

			Zoom(viewportPoint, newZoom);
		}

		private void OnScaleChanged()
		{
			UpdateTransform();
			InvalidateMeasure();
			InvalidatePresenterArrange();
		}

		private void OnScrollClientScrollInfoChanged(object sender, ScrollInfoChangedEventArgs e)
		{
			UpdateTransform();

			OnScrollInfoChanged(e);
		}

		protected void OnScrollInfoChanged(ScrollInfoChangedEventArgs e)
		{
			ScrollInfoChanged?.Invoke(this, e);
		}

		private void UpdatePaddingAndOffset(Vector offset)
		{
			var padding = Padding;

			CalculatePaddingAndOffset(ScrollInfo, ref offset, ref padding);

			Padding = padding;
			Offset = offset;
		}

		internal void UpdatePaddingInternal()
		{
			UpdatePaddingAndOffset(Offset);
		}

		protected void UpdateScale(double scaleX, double scaleY)
		{
			_scaleX = scaleX;
			_scaleY = scaleY;

			OnScaleChanged();
		}

		private void UpdateScrollClient()
		{
			var delegateScrollViewPanel = _child as IDelegateScrollViewPanel;

			ScrollClient = delegateScrollViewPanel?.ScrollViewPanel ?? _child as IScrollViewPanel;
		}

		private void UpdateScrollInfo()
		{
			ScrollInfo = CalculateScrollInfo();
		}

		protected virtual void UpdateTransform()
		{
			if (IsScrollClient == false)
			{
				Transform.ScaleX = 1.0;
				Transform.ScaleY = 1.0;
				Transform.TranslateX = 0.0;
				Transform.TranslateY = 0.0;

				return;
			}

			var scaleX = ScaleX;
			var scaleY = ScaleY;
			var offset = ScrollInfo.ClampOffset(Offset);

			offset = CoerceRoundOffset(offset);

			Transform.TranslateX = -offset.X;
			Transform.TranslateY = -offset.Y;

			Transform.ScaleX = scaleX;
			Transform.ScaleY = scaleY;

			InvalidateArrange();
		}

		private void Zoom(Point viewportPoint, double newZoom)
		{
			var newScaleX = newZoom;
			var newScaleY = newZoom;

			var elementBounds = GetChildBounds();

			elementBounds.Scale(newScaleX, newScaleY, viewportPoint.X, viewportPoint.Y);

			var scrollOffset = ScrollInfo.ClampOffset(Offset);
			var location = elementBounds.TransformedLocation;

			elementBounds = GetChildBounds(scrollOffset.X, scrollOffset.Y, newScaleX, newScaleY);

			elementBounds.RecalculateTranslate(location.X, location.Y);

			_scaleX = newScaleX;
			_scaleY = newScaleY;

			var translate = elementBounds.Translate;
			var offset = new Vector(-translate.X, -translate.Y);
			var padding = new Thickness(0);
			var scrollInfo = CalculateScrollInfo(ActualChildSize, ActualViewport, newScaleX, newScaleY, padding);

			CalculatePaddingAndOffset(scrollInfo, ref offset, ref padding);

			UpdateScrollInfo();

			Padding = padding;
			Offset = offset;

			OnScaleChanged();
		}

		event EventHandler<ScrollInfoChangedEventArgs> IScrollViewPanel.ScrollInfoChanged
		{
			add => ScrollInfoChanged += value;
			remove => ScrollInfoChanged -= value;
		}

		bool IScrollViewPanel.CanHorizontallyScroll
		{
			get => CanHorizontallyScroll;
			set => CanHorizontallyScroll = value;
		}

		bool IScrollViewPanel.CanVerticallyScroll
		{
			get => CanVerticallyScroll;
			set => CanVerticallyScroll = value;
		}

		Size IScrollViewPanel.Extent => Extent;

		Vector IScrollViewPanel.Offset
		{
			get => Offset;
			set => Offset = value;
		}

		Size IScrollViewPanel.Viewport => Viewport;

		void IScrollViewPanel.ExecuteScrollCommand(ScrollCommandKind command)
		{
			ExecuteScrollCommand(command);
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition CanHorizontallyScroll;
			public static readonly PackedBoolItemDefinition CanVerticallyScroll;
			public static readonly PackedBoolItemDefinition ChildMeasured;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				CanHorizontallyScroll = allocator.AllocateBoolItem();
				CanVerticallyScroll = allocator.AllocateBoolItem();
				ChildMeasured = allocator.AllocateBoolItem();
			}
		}
	}
}