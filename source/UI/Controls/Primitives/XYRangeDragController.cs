// <copyright file="XYRangeDragController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Primitives
{
	internal abstract class XYRangeDragController<TControl, TItem>
		where TControl : FrameworkElement
		where TItem : FrameworkElement
	{
		private Point _controlOrigin;
		private Point _controlPosition;
		private TItem _dragItem;
		private double _originX;
		private double _originXPixelRatio;
		private double _originY;
		private double _originYPixelRatio;
		private double _pixelDeltaX;
		private double _pixelDeltaY;
		private double _valueDeltaX;
		private double _valueDeltaY;

		protected XYRangeDragController(TControl control)
		{
			Control = control;
		}

		public TControl Control { get; }

		protected abstract Range<double> RangeX { get; }

		protected abstract Range<double> RangeY { get; }

		protected abstract void DragSyncValue(TItem item, double x, double y);

		private void FinishDrag()
		{
			if (_dragItem == null)
				return;

			var dragItem = _dragItem;

			_dragItem = null;

			Control.ReleaseMouseCapture();

			OnDragEnded(dragItem);
		}

		protected virtual TItem GetDragItem(MouseButtonEventArgs e)
		{
			var sourceElement = e.OriginalSource as UIElement;

			if (sourceElement.IsVisualDescendantOf(Control) == false)
				return null;

			return sourceElement.GetVisualAncestorsAndSelf().TakeWhile(i => ReferenceEquals(i, Control) == false).OfType<TItem>().FirstOrDefault();
		}

		protected abstract double GetPixelRatioX(TItem item);

		protected abstract double GetPixelRatioY(TItem item);

		protected abstract double GetX(TItem item);

		protected abstract double GetY(TItem item);

		protected virtual void OnDragEnded(TItem dragItem)
		{
		}

		protected virtual void OnDragStarted(TItem dragItem)
		{
		}

		internal void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			var item = GetDragItem(e);

			if (item == null)
				return;

			_dragItem = Control.CaptureMouse() ? item : null;

			if (_dragItem == null)
				return;

			_controlOrigin = e.GetPosition(Control);

			_originXPixelRatio = GetPixelRatioX(_dragItem);
			_originYPixelRatio = GetPixelRatioY(_dragItem);

			_controlPosition = _controlOrigin;
			_pixelDeltaX = 0;
			_pixelDeltaY = 0;

			_valueDeltaX = 0;
			_valueDeltaY = 0;

			_originX = GetX(item);
			_originY = GetY(item);

			OnDragStarted(_dragItem);
		}

		internal void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (_dragItem == null)
				return;

			UpdateValueOnMouseEvent(e);

			FinishDrag();
		}

		internal void OnMouseMove(MouseEventArgs e)
		{
			if (_dragItem == null)
				return;

			UpdateValueOnMouseEvent(e);
		}

		private void UpdateValueOnMouseEvent(MouseEventArgs e)
		{
			if (_dragItem == null)
				return;

			_controlPosition = e.GetPosition(Control);

			_pixelDeltaX = _controlPosition.X - _controlOrigin.X;
			_pixelDeltaY = _controlPosition.Y - _controlOrigin.Y;

			_valueDeltaX = _pixelDeltaX / GetPixelRatioX(_dragItem);
			_valueDeltaY = _pixelDeltaY / GetPixelRatioY(_dragItem);

			DragSyncValue(_dragItem, _originX + _valueDeltaX, _originY + _valueDeltaY);
		}
	}
}