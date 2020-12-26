// <copyright file="ScrollViewControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.ScrollView
{
	[ContentProperty(nameof(Child))]
	[DefaultProperty(nameof(Child))]
	[TemplateContractType(typeof(ScrollViewControlBaseTemplateContract))]
	public abstract class ScrollViewControlBase : TemplateContractControl
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<FrameworkElement, ScrollViewControlBase>
			(nameof(Child), s => s.OnChildChangedPrivate);

		private static readonly DependencyPropertyKey ActualHorizontalScrollBarVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, ScrollViewControlBase>
			(nameof(ActualHorizontalScrollBarVisibility), Visibility.Collapsed, s => s.OnActualHorizontalScrollBarVisibilityChangedPrivate);

		public static readonly DependencyProperty ActualHorizontalScrollBarVisibilityProperty = ActualHorizontalScrollBarVisibilityPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualVerticalScrollBarVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, ScrollViewControlBase>
			(nameof(ActualVerticalScrollBarVisibility), Visibility.Collapsed, s => s.OnActualVerticalScrollBarVisibilityChangedPrivate);

		public static readonly DependencyProperty ActualVerticalScrollBarVisibilityProperty = ActualVerticalScrollBarVisibilityPropertyKey.DependencyProperty;

		public static readonly DependencyProperty WheelOrientationProperty = DPM.Register<Orientation, ScrollViewControlBase>
			(nameof(WheelOrientation), Orientation.Vertical, s => s.OnWheelOrientationChangedPrivate);

		public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DPM.RegisterAttached<ScrollBarVisibility, ScrollViewControlBase>
			(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto, OnVerticalScrollBarVisibilityChanged);

		public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DPM.RegisterAttached<ScrollBarVisibility, ScrollViewControlBase>
			(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Auto, OnHorizontalScrollBarVisibilityChanged);

		private static readonly DependencyPropertyKey ScrollableHeightPropertyKey = DPM.RegisterReadOnly<double, ScrollViewControlBase>
			(nameof(ScrollableHeight), s => s.OnScrollableSizeChanged);

		public static readonly DependencyProperty ScrollableHeightProperty = ScrollableHeightPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ViewportHeightPropertyKey = DPM.RegisterReadOnly<double, ScrollViewControlBase>
			(nameof(ViewportHeight), s => s.OnViewportSizeChanged);

		public static readonly DependencyProperty ViewportHeightProperty = ViewportHeightPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ScrollableWidthPropertyKey = DPM.RegisterReadOnly<double, ScrollViewControlBase>
			(nameof(ScrollableWidth), s => s.OnScrollableSizeChanged);

		public static readonly DependencyProperty ScrollableWidthProperty = ScrollableWidthPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ExtentHeightPropertyKey = DPM.RegisterReadOnly<double, ScrollViewControlBase>
			(nameof(ExtentHeight), s => s.OnExtentSizeChanged);

		public static readonly DependencyProperty ExtentHeightProperty = ExtentHeightPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ExtentWidthPropertyKey = DPM.RegisterReadOnly<double, ScrollViewControlBase>
			(nameof(ExtentWidth), s => s.OnExtentSizeChanged);

		private static readonly DependencyPropertyKey ViewportWidthPropertyKey = DPM.RegisterReadOnly<double, ScrollViewControlBase>
			(nameof(ViewportWidth), s => s.OnViewportSizeChanged);

		public static readonly DependencyProperty VerticalOffsetProperty = DPM.Register<double, ScrollViewControlBase>
			(nameof(VerticalOffset), s => s.OnVerticalOffsetChangedPrivate, s => s.CoerceVerticalOffsetPrivate);

		public static readonly DependencyProperty HorizontalOffsetProperty = DPM.Register<double, ScrollViewControlBase>
			(nameof(HorizontalOffset), s => s.OnHorizontalOffsetChangedPrivate, s => s.CoerceHorizontalOffsetPrivate);

		public static readonly DependencyProperty ViewportWidthProperty = ViewportWidthPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ExtentWidthProperty = ExtentWidthPropertyKey.DependencyProperty;

		public static readonly DependencyProperty PreserveScrollBarVisibilityProperty = DPM.Register<bool, ScrollViewControlBase>
			(nameof(PreserveScrollBarVisibility), false);

		private double _horizontalOffsetCache;
		private EventHandler _layoutUpdateHandler;
		private object _logicalChild;
		private byte _packedValue;
		private ScrollViewPanelBase _scrollViewPanel;
		private double _verticalOffsetCache;

		internal ScrollViewControlBase()
		{
		}

		public Visibility ActualHorizontalScrollBarVisibility
		{
			get => (Visibility) GetValue(ActualHorizontalScrollBarVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualHorizontalScrollBarVisibilityPropertyKey, value);
		}

		private IScrollViewPanel ActualScrollViewPanel => ScrollViewPanel ?? DummyScrollViewPanel.Instance;

		public Visibility ActualVerticalScrollBarVisibility
		{
			get => (Visibility) GetValue(ActualVerticalScrollBarVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualVerticalScrollBarVisibilityPropertyKey, value);
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		public double ExtentHeight
		{
			get => (double) GetValue(ExtentHeightProperty);
			private set => this.SetReadOnlyValue(ExtentHeightPropertyKey, value);
		}

		public double ExtentWidth
		{
			get => (double) GetValue(ExtentWidthProperty);
			private set => this.SetReadOnlyValue(ExtentWidthPropertyKey, value);
		}

		public double HorizontalOffset
		{
			get => (double) GetValue(HorizontalOffsetProperty);
			set => SetValue(HorizontalOffsetProperty, value);
		}

		protected ScrollBar HorizontalScrollBar => TemplateContract.HorizontalScrollBar;

		private bool HorizontalScrollBarShown
		{
			get => PackedDefinition.HorizontalScrollBarShown.GetValue(_packedValue);
			set => PackedDefinition.HorizontalScrollBarShown.SetValue(ref _packedValue, value);
		}

		public ScrollBarVisibility HorizontalScrollBarVisibility
		{
			get => (ScrollBarVisibility) GetValue(HorizontalScrollBarVisibilityProperty);
			set => SetValue(HorizontalScrollBarVisibilityProperty, value);
		}

		private bool IsHorizontalWheel
		{
			get => PackedDefinition.IsHorizontalWheel.GetValue(_packedValue);
			set => PackedDefinition.IsHorizontalWheel.SetValue(ref _packedValue, value);
		}

		protected bool IsInArrange
		{
			get => PackedDefinition.IsInArrange.GetValue(_packedValue);
			private set => PackedDefinition.IsInArrange.SetValue(ref _packedValue, value);
		}

		protected bool IsInMeasure
		{
			get => PackedDefinition.IsInMeasure.GetValue(_packedValue);
			private set => PackedDefinition.IsInMeasure.SetValue(ref _packedValue, value);
		}

		protected bool IsInScrollCommand
		{
			get => PackedDefinition.IsInScrollCommand.GetValue(_packedValue);
			private set => PackedDefinition.IsInScrollCommand.SetValue(ref _packedValue, value);
		}

		internal object LogicalChild
		{
			get => _logicalChild;
			set
			{
				if (ReferenceEquals(_logicalChild, value))
					return;

				if (_logicalChild != null)
					RemoveLogicalChild(_logicalChild);

				_logicalChild = value;

				if (_logicalChild != null)
					AddLogicalChild(_logicalChild);
			}
		}

		protected override IEnumerator LogicalChildren => LogicalChild != null ? EnumeratorUtils.Concat(LogicalChild, base.LogicalChildren) : base.LogicalChildren;

		private protected virtual bool PreserveHorizontalOffset => true;

		public bool PreserveScrollBarVisibility
		{
			get => (bool) GetValue(PreserveScrollBarVisibilityProperty);
			set => SetValue(PreserveScrollBarVisibilityProperty, value);
		}

		private protected virtual bool PreserveVerticalOffset => true;

		public double ScrollableHeight
		{
			get => (double) GetValue(ScrollableHeightProperty);
			private set => this.SetReadOnlyValue(ScrollableHeightPropertyKey, value);
		}

		public double ScrollableWidth
		{
			get => (double) GetValue(ScrollableWidthProperty);
			private set => this.SetReadOnlyValue(ScrollableWidthPropertyKey, value);
		}

		private ScrollInfo ScrollInfo { get; set; }

		protected ScrollViewPanelBase ScrollViewPanel
		{
			get => _scrollViewPanel;
			private set
			{
				if (ReferenceEquals(ScrollViewPanel, value))
					return;

				if (_scrollViewPanel != null)
				{
					_scrollViewPanel.ScrollInfoChanged -= OnScrollInfoChanged;
					_scrollViewPanel.OffsetChanged -= OnOffsetChanged;
				}

				_scrollViewPanel = value;

				if (_scrollViewPanel != null)
				{
					UpdateCanHorizontallyScroll();
					UpdateCanVerticallyScroll();
					UpdateOffset();

					_scrollViewPanel.ScrollInfoChanged += OnScrollInfoChanged;
					_scrollViewPanel.OffsetChanged += OnOffsetChanged;
				}

				UpdateScroll();
			}
		}

		protected abstract ScrollViewPanelBase ScrollViewPanelCore { get; }

		private bool SuspendOffsetHandler
		{
			get => PackedDefinition.SuspendOffsetHandler.GetValue(_packedValue);
			set => PackedDefinition.SuspendOffsetHandler.SetValue(ref _packedValue, value);
		}

		private ScrollViewControlBaseTemplateContract TemplateContract => (ScrollViewControlBaseTemplateContract) TemplateContractInternal;

		public double VerticalOffset
		{
			get => (double) GetValue(VerticalOffsetProperty);
			set => SetValue(VerticalOffsetProperty, value);
		}

		protected ScrollBar VerticalScrollBar => TemplateContract.VerticalScrollBar;

		private bool VerticalScrollBarShown
		{
			get => PackedDefinition.VerticalScrollBarShown.GetValue(_packedValue);
			set => PackedDefinition.VerticalScrollBarShown.SetValue(ref _packedValue, value);
		}

		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get => (ScrollBarVisibility) GetValue(VerticalScrollBarVisibilityProperty);
			set => SetValue(VerticalScrollBarVisibilityProperty, value);
		}

		public double ViewportHeight
		{
			get => (double) GetValue(ViewportHeightProperty);
			private set => this.SetReadOnlyValue(ViewportHeightPropertyKey, value);
		}

		public double ViewportWidth
		{
			get => (double) GetValue(ViewportWidthProperty);
			private set => this.SetReadOnlyValue(ViewportWidthPropertyKey, value);
		}

		public Orientation WheelOrientation
		{
			get => (Orientation) GetValue(WheelOrientationProperty);
			set => SetValue(WheelOrientationProperty, value);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			try
			{
				IsInArrange = true;

				return base.ArrangeOverride(arrangeBounds);
			}
			finally
			{
				IsInArrange = false;
			}
		}

		protected virtual Visibility CalculateHorizontalScrollBarVisibility()
		{
			if (PreserveScrollBarVisibility && HorizontalScrollBarShown)
				return Visibility.Visible;

			var visibility = HorizontalScrollBarVisibility;

			if (visibility == ScrollBarVisibility.Disabled || visibility == ScrollBarVisibility.Hidden)
				return Visibility.Collapsed;

			if (visibility == ScrollBarVisibility.Auto)
				return ExtentWidth > ViewportWidth ? Visibility.Visible : Visibility.Collapsed;

			return Visibility.Visible;
		}

		protected virtual Visibility CalculateVerticalScrollBarVisibility()
		{
			if (PreserveScrollBarVisibility && VerticalScrollBarShown)
				return Visibility.Visible;

			var visibility = VerticalScrollBarVisibility;

			if (visibility == ScrollBarVisibility.Disabled || visibility == ScrollBarVisibility.Hidden)
				return Visibility.Collapsed;

			if (visibility == ScrollBarVisibility.Auto)
				return ExtentHeight > ViewportHeight ? Visibility.Visible : Visibility.Collapsed;

			return Visibility.Visible;
		}

		private protected virtual double CoerceHorizontalOffset(double offset)
		{
			return offset;
		}

		private object CoerceHorizontalOffsetPrivate(object arg)
		{
			var coerced = CoerceHorizontalOffset((double) arg).Clamp(0, ScrollableWidth);

			return coerced.Equals(arg) ? arg : coerced;
		}

		private protected virtual double CoerceVerticalOffset(double offset)
		{
			return offset;
		}

		private object CoerceVerticalOffsetPrivate(object arg)
		{
			var coerced = CoerceVerticalOffset((double) arg).Clamp(0, ScrollableHeight);

			return coerced.Equals(arg) ? arg : coerced;
		}

		private void EnsureLayoutUpdateHandler()
		{
			if (_layoutUpdateHandler != null)
				return;

			_layoutUpdateHandler = OnLayoutUpdate;

			LayoutUpdated += _layoutUpdateHandler;
		}

		protected virtual void EnterScrollCommand()
		{
			IsInScrollCommand = true;
		}

		public void ExecuteScrollCommand(ScrollCommandKind scrollCommandKind)
		{
			ExecuteScrollCommandCore(scrollCommandKind);
		}

		protected virtual void ExecuteScrollCommandCore(ScrollCommandKind scrollCommandKind)
		{
			try
			{
				EnterScrollCommand();

				SyncScrollOffset();

				ScrollViewPanel?.ExecuteScrollCommand(scrollCommandKind);

				UpdateScrollOffsetCache(ScrollViewUtils.GetCommandOrientation(scrollCommandKind));
			}
			finally
			{
				LeaveScrollCommand();
			}
		}

		internal ElementBounds? GetChildBoundsInternal(bool round)
		{
			return ScrollViewPanel?.GetChildBoundsInternal(round);
		}

		public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject dependencyObject)
		{
			return (ScrollBarVisibility) dependencyObject.GetValue(HorizontalScrollBarVisibilityProperty);
		}

		public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject dependencyObject)
		{
			return (ScrollBarVisibility) dependencyObject.GetValue(VerticalScrollBarVisibilityProperty);
		}

		internal bool HandleKeyDown(Key key, Orientation orientation)
		{
			return false;
		}

		//private protected abstract void InvalidatePanelMeasure();

		protected void InvalidateScroll()
		{
			if (IsInMeasure || IsInScrollCommand)
				UpdateScroll();
			else
				InvalidateMeasure();
		}

		protected virtual void LeaveScrollCommand()
		{
			IsInScrollCommand = false;
		}

		public void LineDown()
		{
			ExecuteScrollCommand(ScrollCommandKind.LineDown);
		}

		public void LineLeft()
		{
			ExecuteScrollCommand(ScrollCommandKind.LineLeft);
		}

		public void LineRight()
		{
			ExecuteScrollCommand(ScrollCommandKind.LineRight);
		}

		public void LineUp()
		{
			ExecuteScrollCommand(ScrollCommandKind.LineUp);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			try
			{
				IsInMeasure = true;

				UpdateScrollViewPanelInternal();
				EnsureLayoutUpdateHandler();
				UpdateScroll();

				var layoutContext = LayoutContext.GetContext(this);

				var actualHorizontalScrollBarVisibility = ActualHorizontalScrollBarVisibility;
				var actualVerticalScrollBarVisibility = ActualVerticalScrollBarVisibility;

				var result = base.MeasureOverride(availableSize);

				if (layoutContext == null || !(layoutContext.MeasureContextPass < MeasureContextPass.FinalMeasureDirty))
				{
					var finalHorizontalScrollBarVisibility = CalculateHorizontalScrollBarVisibility();
					var finalVerticalScrollBarVisibility = CalculateVerticalScrollBarVisibility();

					var remeasure = false;

					if (actualHorizontalScrollBarVisibility != finalHorizontalScrollBarVisibility)
					{
						UpdateHorizontalScrollBarVisibility(finalHorizontalScrollBarVisibility, true);

						remeasure = true;
					}

					if (actualVerticalScrollBarVisibility != finalVerticalScrollBarVisibility)
					{
						UpdateVerticalScrollBarVisibility(finalVerticalScrollBarVisibility, true);

						remeasure = true;
					}

					if (remeasure)
						result = base.MeasureOverride(availableSize);

					finalHorizontalScrollBarVisibility = CalculateHorizontalScrollBarVisibility();
					finalVerticalScrollBarVisibility = CalculateVerticalScrollBarVisibility();

					if (finalHorizontalScrollBarVisibility == actualHorizontalScrollBarVisibility && actualHorizontalScrollBarVisibility == Visibility.Visible)
					{
						UpdateHorizontalScrollBarVisibility(finalHorizontalScrollBarVisibility, true);

						remeasure = true;
					}

					if (finalVerticalScrollBarVisibility == actualVerticalScrollBarVisibility && actualVerticalScrollBarVisibility == Visibility.Visible)
					{
						UpdateVerticalScrollBarVisibility(finalVerticalScrollBarVisibility, true);

						remeasure = true;
					}

					if (remeasure)
						result = base.MeasureOverride(availableSize);

					return result;
				}

				if (layoutContext.MeasureContextPass < MeasureContextPass.FinalMeasureDirty)
				{
					var finalHorizontalScrollBarVisibility = CalculateHorizontalScrollBarVisibility();
					var finalVerticalScrollBarVisibility = CalculateVerticalScrollBarVisibility();

					var remeasure = false;

					if (actualHorizontalScrollBarVisibility != finalHorizontalScrollBarVisibility)
					{
						UpdateHorizontalScrollBarVisibility(finalHorizontalScrollBarVisibility, true);

						remeasure = true;
					}

					if (actualVerticalScrollBarVisibility != finalVerticalScrollBarVisibility)
					{
						UpdateVerticalScrollBarVisibility(finalVerticalScrollBarVisibility, true);

						remeasure = true;
					}

					if (remeasure)
					{
						layoutContext.OnDescendantMeasureDirty(this);
					}
				}

				return result;
			}
			finally
			{
				IsInMeasure = false;
			}
		}

		private void OnActualHorizontalScrollBarVisibilityChangedPrivate(Visibility oldVisibility, Visibility newVisibility)
		{
			if (newVisibility == Visibility.Visible)
				HorizontalScrollBarShown = true;
		}

		private void OnActualVerticalScrollBarVisibilityChangedPrivate(Visibility oldVisibility, Visibility newVisibility)
		{
			if (newVisibility == Visibility.Visible)
				VerticalScrollBarShown = true;
		}

		internal virtual void OnChildChangedInternal(FrameworkElement oldChild, FrameworkElement newChild)
		{
			UpdateScrollViewPanelInternal();
		}

		private void OnChildChangedPrivate(FrameworkElement oldChild, FrameworkElement newChild)
		{
			OnChildChangedInternal(oldChild, newChild);
		}

		private void OnExtentSizeChanged()
		{
		}

		private protected virtual void OnHorizontalOffsetChangedInternal()
		{
		}

		private void OnHorizontalOffsetChangedPrivate(double oldOffset, double newOffset)
		{
			if (SuspendOffsetHandler)
			{
				OnHorizontalOffsetChangedInternal();

				return;
			}

			_horizontalOffsetCache = newOffset;

			SyncScrollOffset();
			OnHorizontalOffsetChangedInternal();
		}

		private static void OnHorizontalScrollBarVisibilityChanged(DependencyObject dependencyObject)
		{
			if (dependencyObject is ScrollViewControl scrollView)
			{
				scrollView.UpdateCanHorizontallyScroll();
				scrollView.UpdateHorizontalScrollBarVisibility(null, true);
			}
		}

		private void OnLayoutUpdate(object sender, EventArgs e)
		{
			//UpdateVerticalScrollBarVisibility();
			//UpdateHorizontalScrollBarVisibility();
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			if (e.Handled)
				return;

			if (IsHorizontalWheel)
				ExecuteScrollCommand(e.Delta < 0 ? ScrollCommandKind.MouseWheelLeft : ScrollCommandKind.MouseWheelRight);
			else
				ExecuteScrollCommand(e.Delta < 0 ? ScrollCommandKind.MouseWheelDown : ScrollCommandKind.MouseWheelUp);
		}

		private void OnOffsetChanged(object sender, OffsetChangedEventArgs e)
		{
			if (SuspendOffsetHandler)
				return;

			try
			{
				SuspendOffsetHandler = true;

				this.SetCurrentValueInternal(HorizontalOffsetProperty, e.NewOffset.X);
				this.SetCurrentValueInternal(VerticalOffsetProperty, e.NewOffset.Y);
			}
			finally
			{
				SuspendOffsetHandler = false;
			}
		}

		private void OnScrollableSizeChanged()
		{
		}

		protected virtual void OnScrollBarDragCompleted(ScrollBar scrollBar)
		{
		}

		internal void OnScrollBarDragCompletedInternal(ScrollBar scrollBar)
		{
			UpdateScrollOffsetCache(scrollBar.Orientation);

			OnScrollBarDragCompleted(scrollBar);
		}

		private void OnScrollInfoChanged(object sender, ScrollInfoChangedEventArgs e)
		{
			InvalidateScroll();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			VerticalScrollBar.IsInScrollViewer = true;
			HorizontalScrollBar.IsInScrollViewer = true;
		}

		protected override void OnTemplateContractDetaching()
		{
			VerticalScrollBar.IsInScrollViewer = false;
			HorizontalScrollBar.IsInScrollViewer = false;

			base.OnTemplateContractDetaching();
		}

		private protected virtual void OnVerticalOffsetChangedInternal()
		{
		}

		private void OnVerticalOffsetChangedPrivate(double oldOffset, double newOffset)
		{
			if (SuspendOffsetHandler)
			{
				OnVerticalOffsetChangedInternal();

				return;
			}

			_verticalOffsetCache = newOffset;

			SyncScrollOffset();
			OnVerticalOffsetChangedInternal();
		}

		private static void OnVerticalScrollBarVisibilityChanged(DependencyObject dependencyObject)
		{
			if (dependencyObject is ScrollViewControl scrollView)
			{
				scrollView.UpdateCanVerticallyScroll();
				scrollView.UpdateVerticalScrollBarVisibility(null, true);
			}
		}

		private void OnViewportSizeChanged()
		{
		}

		protected virtual void OnWheelOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
		{
			IsHorizontalWheel = newOrientation == Orientation.Vertical;
		}

		private void OnWheelOrientationChangedPrivate(Orientation oldOrientation, Orientation newOrientation)
		{
			OnWheelOrientationChanged(oldOrientation, newOrientation);
		}

		public void PageDown()
		{
			ExecuteScrollCommand(ScrollCommandKind.PageDown);
		}

		public void PageLeft()
		{
			ExecuteScrollCommand(ScrollCommandKind.PageLeft);
		}

		public void PageRight()
		{
			ExecuteScrollCommand(ScrollCommandKind.PageRight);
		}

		public void PageUp()
		{
			ExecuteScrollCommand(ScrollCommandKind.PageUp);
		}

		internal void ResetScrollBarVisibilityShown()
		{
			VerticalScrollBarShown = false;
			HorizontalScrollBarShown = false;
		}

		public void ScrollToBottom()
		{
			ExecuteScrollCommand(ScrollCommandKind.ScrollToBottom);
		}

		public void ScrollToLeft()
		{
			ExecuteScrollCommand(ScrollCommandKind.ScrollToLeft);
		}

		public void ScrollToRight()
		{
			ExecuteScrollCommand(ScrollCommandKind.ScrollToRight);
		}

		public void ScrollToTop()
		{
			ExecuteScrollCommand(ScrollCommandKind.ScrollToTop);
		}

		public static void SetHorizontalScrollBarVisibility(DependencyObject dependencyObject, ScrollBarVisibility visibility)
		{
			dependencyObject.SetValue(HorizontalScrollBarVisibilityProperty, visibility);
		}

		public static void SetVerticalScrollBarVisibility(DependencyObject dependencyObject, ScrollBarVisibility visibility)
		{
			dependencyObject.SetValue(VerticalScrollBarVisibilityProperty, visibility);
		}

		private void SyncScrollOffset()
		{
			if (SuspendOffsetHandler)
				return;

			SuspendOffsetHandler = true;

			try
			{
				ActualScrollViewPanel.Offset = new Vector(PreserveHorizontalOffset ? _horizontalOffsetCache : HorizontalOffset, PreserveVerticalOffset ? _verticalOffsetCache : VerticalOffset);
			}
			finally
			{
				SuspendOffsetHandler = false;
			}
		}

		private void UpdateCanHorizontallyScroll()
		{
			ActualScrollViewPanel.CanHorizontallyScroll = HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
		}

		private void UpdateCanVerticallyScroll()
		{
			ActualScrollViewPanel.CanVerticallyScroll = VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
		}

		protected void UpdateHorizontalScrollBarVisibility(Visibility? visibility = null, bool invalidateMeasure = false)
		{
			ActualHorizontalScrollBarVisibility = visibility ?? CalculateHorizontalScrollBarVisibility();

			if (invalidateMeasure == false)
				return;

			if (HorizontalScrollBar != null)
			{
				HorizontalScrollBar.InvalidateMeasure();
				HorizontalScrollBar.InvalidateAncestorsMeasure(this);
			}
		}

		private void UpdateOffset()
		{
			ActualScrollViewPanel.Offset = new Vector(HorizontalOffset, VerticalOffset);
		}

		protected virtual void UpdateScroll()
		{
			try
			{
				SuspendOffsetHandler = true;

				var scrollViewerPanel = ActualScrollViewPanel;
				var oldScrollInfo = ScrollInfo;
				var newScrollInfo = new ScrollInfo
				{
					Extent = scrollViewerPanel.Extent,
					Viewport = scrollViewerPanel.Viewport
				};

				var newOffset = scrollViewerPanel.Offset;

				if (oldScrollInfo.Extent.Height.Equals(newScrollInfo.Extent.Height) == false)
					ExtentHeight = newScrollInfo.Extent.Height;

				if (oldScrollInfo.Extent.Width.Equals(newScrollInfo.Extent.Width) == false)
					ExtentWidth = newScrollInfo.Extent.Width;

				if (oldScrollInfo.Viewport.Height.Equals(newScrollInfo.Viewport.Height) == false)
					ViewportHeight = newScrollInfo.Viewport.Height;

				if (oldScrollInfo.Viewport.Width.Equals(newScrollInfo.Viewport.Width) == false)
					ViewportWidth = newScrollInfo.Viewport.Width;

				HorizontalOffset = newOffset.X;
				VerticalOffset = newOffset.Y;

				var scrollableSize = newScrollInfo.ScrollableSize;
				var currentScrollableSize = oldScrollInfo.ScrollableSize;

				if (scrollableSize.Height.Equals(currentScrollableSize.Height) == false)
					ScrollableHeight = scrollableSize.Height;

				if (scrollableSize.Width.Equals(currentScrollableSize.Width) == false)
					ScrollableWidth = scrollableSize.Width;

				ScrollInfo = newScrollInfo;
			}
			finally
			{
				SuspendOffsetHandler = false;
			}
		}

		private void UpdateScrollOffsetCache(Orientation? orientation)
		{
			if (orientation == Orientation.Horizontal)
				_horizontalOffsetCache = HorizontalOffset;
			else
				_verticalOffsetCache = VerticalOffset;
		}

		private void UpdateScrollOffsetCache()
		{
			_horizontalOffsetCache = HorizontalOffset;
			_verticalOffsetCache = VerticalOffset;
		}

		internal void UpdateScrollOffsetCacheInternal(Orientation orientation)
		{
			UpdateScrollOffsetCache(orientation);
		}

		internal void UpdateScrollOffsetCacheInternal()
		{
			UpdateScrollOffsetCache();
		}

		private void UpdateScrollViewPanel()
		{
			ScrollViewPanel = ScrollViewPanelCore;
		}

		internal void UpdateScrollViewPanelInternal()
		{
			UpdateScrollViewPanel();
		}

		protected void UpdateVerticalScrollBarVisibility(Visibility? visibility = null, bool invalidateMeasure = false)
		{
			ActualVerticalScrollBarVisibility = visibility ?? CalculateVerticalScrollBarVisibility();

			if (invalidateMeasure == false)
				return;

			if (VerticalScrollBar != null)
			{
				VerticalScrollBar.InvalidateMeasure();
				VerticalScrollBar.InvalidateAncestorsMeasure(this);
			}
		}

		private class DummyScrollViewPanel : IScrollViewPanel
		{
			public static readonly IScrollViewPanel Instance = new DummyScrollViewPanel();

			private DummyScrollViewPanel()
			{
			}

			public event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged
			{
				add { }
				remove { }
			}

			public event EventHandler<OffsetChangedEventArgs> OffsetChanged
			{
				add { }
				remove { }
			}

			public bool CanHorizontallyScroll { get; set; }

			public bool CanVerticallyScroll { get; set; }

			public Size Extent { get; } = new Size();

			public Vector Offset { get; set; } = new Vector();

			public Size Viewport { get; } = new Size();

			public void ExecuteScrollCommand(ScrollCommandKind command)
			{
			}
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsInMeasure;
			public static readonly PackedBoolItemDefinition IsInArrange;
			public static readonly PackedBoolItemDefinition IsInScrollCommand;
			public static readonly PackedBoolItemDefinition SuspendOffsetHandler;
			public static readonly PackedBoolItemDefinition IsHorizontalWheel;

			public static readonly PackedBoolItemDefinition HorizontalScrollBarShown;
			public static readonly PackedBoolItemDefinition VerticalScrollBarShown;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsInMeasure = allocator.AllocateBoolItem();
				IsInArrange = allocator.AllocateBoolItem();
				IsInScrollCommand = allocator.AllocateBoolItem();
				SuspendOffsetHandler = allocator.AllocateBoolItem();
				IsHorizontalWheel = allocator.AllocateBoolItem();
				HorizontalScrollBarShown = allocator.AllocateBoolItem();
				VerticalScrollBarShown = allocator.AllocateBoolItem();
			}
		}
	}

	public abstract class ScrollViewControlBase<TScrollViewPresenter, TScrollContentPanel> : ScrollViewControlBase where TScrollViewPresenter : ScrollViewPresenterBase<TScrollContentPanel> where TScrollContentPanel : ScrollViewPanelBase
	{
		internal ScrollViewControlBase()
		{
		}

		internal TScrollViewPresenter ScrollViewPresenterInternal => TemplateContract.ScrollViewPresenterInternal;

		private ScrollViewControlBaseTemplateContract<TScrollViewPresenter, TScrollContentPanel> TemplateContract => (ScrollViewControlBaseTemplateContract<TScrollViewPresenter, TScrollContentPanel>) TemplateContractInternal;

		internal override void OnChildChangedInternal(FrameworkElement oldChild, FrameworkElement newChild)
		{
			if (TemplateContract.IsAttached == false)
				LogicalChild = newChild;
			else
			{
				if (ScrollViewPresenterInternal != null)
					ScrollViewPresenterInternal.Child = Child;
			}
		}
	}

	public abstract class ScrollViewControlBaseTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ScrollBar HorizontalScrollBar { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ScrollBar VerticalScrollBar { get; [UsedImplicitly] private set; }
	}

	public abstract class ScrollViewControlBaseTemplateContract<TScrollViewPresenter, TScrollContentPanel> : ScrollViewControlBaseTemplateContract
		where TScrollViewPresenter : ScrollViewPresenterBase<TScrollContentPanel> where TScrollContentPanel : ScrollViewPanelBase
	{
		protected abstract TScrollViewPresenter ScrollViewPresenterCore { get; }

		internal TScrollViewPresenter ScrollViewPresenterInternal => ScrollViewPresenterCore;
	}
}