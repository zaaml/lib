// <copyright file="PopupPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed partial class PopupPanel : SingleChildPanel, ILayoutContextPanel
	{
		private Point _calculatedOffset;
		private Rect _calculatedRect;
		private bool _firstMeasure = true;
		private Size _lastMeasureSize;
		private Point _offset;
		private Popup _popup;

		private Point _popupCoerceOffset;

		internal PopupPanel(Popup popup)
		{
			Popup = popup;
			LayoutContext = new PopupLayoutContext(this);
			UseLayoutRounding = true;
		}

		internal Point ActualOffset => _calculatedOffset.WithOffset(Offset);

		internal Size ActualSize { get; private set; }

		internal bool IsMouseVisited { get; private set; }

		private PopupLayoutContext LayoutContext { get; }

		internal Point Offset
		{
			get => _offset;
			set
			{
				if (_offset.IsCloseTo(value))
					return;

				_offset = value;

				ApplyOffset();
			}
		}

		internal Popup Popup
		{
			get => _popup;
			set
			{
				if (ReferenceEquals(_popup, value))
					return;

				_popup = value;

				if (_popup != null)
					this.BindProperties(BackgroundProperty, _popup, Control.BackgroundProperty);
				else
					ClearValue(BackgroundProperty);

				InvalidateMeasure();
			}
		}

		private PopupSource PopupSource => Popup?.PopupSource;

		LayoutContext ILayoutContextPanel.Context => LayoutContext;

		internal event EventHandler ArrangedInternal;

		private void ApplyOffset()
		{
			if (Popup == null || Popup.IsHidden)
				return;

			var calculatedOffset = _calculatedOffset.WithOffset(Offset);

			if (PopupSource.HorizontalOffset.IsCloseTo(calculatedOffset.X, XamlConstants.LayoutComparisonPrecision) == false)
				PopupSource.HorizontalOffset = calculatedOffset.X;

			if (PopupSource.VerticalOffset.IsCloseTo(calculatedOffset.Y, XamlConstants.LayoutComparisonPrecision) == false)
				PopupSource.VerticalOffset = calculatedOffset.Y;
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			if (Popup?.IsOpen != true)
				return XamlConstants.ZeroSize;

			var finalRect = finalSize.Rect().WithOffset(_popupCoerceOffset);

			finalRect.Width += Math.Abs(_popupCoerceOffset.X);
			finalRect.Height += Math.Abs(_popupCoerceOffset.Y);

			var desiredSize = new Size();
			var child = Child;

			if (child != null)
			{
				child.Arrange(finalRect);
				desiredSize = child.DesiredSize;
			}

			ActualSize = finalSize;

			ApplyOffset();

			ArrangedInternal?.Invoke(this, EventArgs.Empty);

			return finalSize.Clamp(new Size(), desiredSize);
		}

		internal Thickness CalcInflate()
		{
			return CalcInflate(Child);
		}

		internal static Thickness CalcInflate(UIElement element)
		{
			return element is FrameworkElement freChild ? Popup.GetInflate(freChild).GetExtended(freChild.Margin) : new Thickness();
		}

		private static Point CoercePopupOffset(Point point)
		{
			if (point.X.IsNaN())
				point.X = 0.0;

			if (point.Y.IsNaN())
				point.Y = 0.0;

			return point;
		}

		private static Point CoerceWPFPopupBounds(ref Rect finalRect, Rect physicalScreenBounds)
		{
			// WPF popup can not be arranged outside of physical screen, it screen position will be automatically adjusted to be within screen,
			// in this case we need to calculate additional popup content offset to workaround popup reposition.

			var popupCoerceOffset = new Point();

			if (finalRect.Left < physicalScreenBounds.Left)
				popupCoerceOffset.X = finalRect.Left - physicalScreenBounds.Left;
			else if (finalRect.Right > physicalScreenBounds.Right)
				popupCoerceOffset.X = finalRect.Right - physicalScreenBounds.Right;
			else
				popupCoerceOffset.X = 0;

			if (finalRect.Top < physicalScreenBounds.Top)
				popupCoerceOffset.Y = finalRect.Top - physicalScreenBounds.Top;
			else if (finalRect.Bottom > physicalScreenBounds.Bottom)
				popupCoerceOffset.Y = finalRect.Bottom - physicalScreenBounds.Bottom;
			else
				popupCoerceOffset.Y = 0;

			var dx = Math.Abs(popupCoerceOffset.X);
			var dy = Math.Abs(popupCoerceOffset.Y);

			finalRect.Width -= dx;
			finalRect.Height -= dy;

			if (popupCoerceOffset.X > 0)
				popupCoerceOffset.X = 0;

			if (popupCoerceOffset.Y > 0)
				popupCoerceOffset.Y = 0;

			return popupCoerceOffset;
		}

		internal void InvalidatePlacement()
		{
			InvalidateMeasure();
		}

		internal bool IsWithinPopup(Point screePoint)
		{
			var screenBox = this.GetScreenLogicalBox();
			var inflated = screenBox.GetInflated(CalcInflate(Child).Negate());

			return inflated.Contains(screePoint);
		}

		private void MeasureChild(UIElement child, Size size, bool force)
		{
			// TODO Review measure size cache (Docking compass measured incorrectly after window maximization)
			//if (child.IsMeasureValid && force == false && (size.Equals(XamlConstants.InfiniteSize) || size.Equals(_lastMeasureSize)))
			//	return;

			_lastMeasureSize = size;

			child.Measure(size);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			try
			{
				LayoutContext.MeasureContextPassCore = MeasureContextPass.MeasureDirty;

				if (Popup?.IsOpen != true)
					return XamlConstants.ZeroSize;

				var child = Child;

				if (child == null)
					return new Size();

				if (_firstMeasure)
					_firstMeasure = false;

				var measureContext = new MeasureContext(this, new Size());

				LayoutContext.MaxAvailableSizeCore = measureContext.ScreenBounds.Size;
				LayoutContext.MeasureContextPassCore = MeasureContextPass.MeasureToContent;

				MeasureChild(child, XamlConstants.InfiniteSize, false);

				LayoutContext.UpdateInvalidElements();

				measureContext = new MeasureContext(this, child.DesiredSize);

				LayoutContext.MeasureContextPassCore = MeasureContextPass.PreviewMeasure;

				var result = MeasurePass(ref measureContext);

				if (LayoutContext.IsMeasureInvalidated)
				{
					LayoutContext.UpdateInvalidElements();

					LayoutContext.MeasureContextPassCore = MeasureContextPass.MeasureToContent;

					MeasureChild(child, XamlConstants.InfiniteSize, true);

					LayoutContext.MeasureContextPassCore = MeasureContextPass.FinalMeasure;

					measureContext = new MeasureContext(this, child.DesiredSize);

					result = MeasurePass(ref measureContext);
				}

				return result;
			}
			finally
			{
				LayoutContext.MeasureContextPassCore = MeasureContextPass.MeasureClean;
			}
		}

		private Size MeasurePass(ref MeasureContext measureContext)
		{
			const int maxPassCount = 3;

			for (var i = 0; i < maxPassCount; i++)
			{
				var child = Child;
				var childDesiredSize = measureContext.DesiredContentSize;
				var arrangeSize = childDesiredSize.Rect().GetInflated(measureContext.Inflate.Negate()).Size();
				var arrangeRect = measureContext.PopupPlacement.Arrange(arrangeSize);

				if (arrangeRect.IsEmpty)
				{
					_calculatedRect = new Rect();
					_popupCoerceOffset = new Point();

					return _calculatedRect.Size();
				}

				var calculatedRect = arrangeRect.GetInflated(measureContext.Inflate);
				var finalMeasureSize = calculatedRect.Size();

				MeasureChild(child, finalMeasureSize, false);

				if (i + 1 < maxPassCount && child.DesiredSize.IsCloseTo(measureContext.DesiredContentSize) == false)
				{
					measureContext = new MeasureContext(this, child.DesiredSize);

					continue;
				}

				_popupCoerceOffset = CoerceWPFPopupBounds(ref calculatedRect, measureContext.ScreenBounds);
				_calculatedOffset = CoercePopupOffset(calculatedRect.GetTopLeft().Round());
				_calculatedRect = calculatedRect;

				break;
			}

			return _calculatedRect.Size();
		}

		[UsedImplicitly]
		private bool NeedChangeOffset()
		{
			if (PopupSource.HorizontalOffset.IsCloseTo(_calculatedOffset.X, XamlConstants.LayoutComparisonPrecision) == false)
				return true;

			if (PopupSource.VerticalOffset.IsCloseTo(_calculatedOffset.Y, XamlConstants.LayoutComparisonPrecision) == false)
				return true;

			return false;
		}

		internal void OnIsHiddenChanged()
		{
			if (Popup.IsHidden)
			{
				Opacity = 0;
				IsHitTestVisible = false;
			}
			else
			{
				Opacity = 1;
				IsHitTestVisible = true;
			}
		}

		private void OnMouseEvent()
		{
			if (Popup.IsOpen)
				IsMouseVisited = true;
		}

		internal void OnPopupClosed()
		{
			IsMouseVisited = false;
			_firstMeasure = true;
			_lastMeasureSize = new Size();

			Child?.InvalidateMeasure();

			InvalidateMeasure();
		}

		private readonly struct MeasureContext
		{
			public MeasureContext(PopupPanel panel, Size desiredContentSize)
			{
				PopupPlacement = panel.Popup.ActualPlacement;
				DesiredContentSize = desiredContentSize;
				Inflate = panel.CalcInflate();
				ScreenBounds = PopupPlacement.ScreenBoundsCore;
			}

			public readonly Size DesiredContentSize;
			public readonly PopupPlacement PopupPlacement;
			public readonly Rect ScreenBounds;
			public readonly Thickness Inflate;
		}

		private sealed class PopupLayoutContext : LayoutContext
		{
			private readonly List<UIElement> _invalidMeasureElements = new();
			private MeasureContextPass _measureContextPassCore;

			public PopupLayoutContext(PopupPanel popupPanel)
			{
				PopupPanel = popupPanel;
			}

			public override ArrangeContextPass ArrangeContextPass => ArrangeContextPassCore;

			public ArrangeContextPass ArrangeContextPassCore { get; }

			public bool IsMeasureInvalidated { get; set; }

			public override Size MaxAvailableSize => MaxAvailableSizeCore;

			public Size MaxAvailableSizeCore { get; set; }

			public override MeasureContextPass MeasureContextPass => MeasureContextPassCore;

			public MeasureContextPass MeasureContextPassCore
			{
				get => _measureContextPassCore;
				set
				{
					if (_measureContextPassCore == value)
						return;

					_measureContextPassCore = value;

					IsMeasureInvalidated = false;
				}
			}

			public PopupPanel PopupPanel { get; }

			public override void OnDescendantMeasureDirty(UIElement element)
			{
				base.OnDescendantMeasureDirty(element);

				if (MeasureContextPassCore < MeasureContextPass.FinalMeasure)
				{
					IsMeasureInvalidated = true;

					_invalidMeasureElements.Add(element);
				}
			}

			public void UpdateInvalidElements()
			{
				foreach (var invalidMeasureElement in _invalidMeasureElements)
					invalidMeasureElement.InvalidateAncestorsMeasure(PopupPanel);

				_invalidMeasureElements.Clear();
			}
		}
	}
}