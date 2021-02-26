// <copyright file="ArtboardResizableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardResizableBehavior : ResizableBehavior
	{
		private ArtboardSnapEngineContext _snapEngineContext;
		internal event EventHandler<CancelEventArgs> ResizeStarting;

		private protected override FrameworkElement ActualElement => Adorner?.AdornedElement ?? FrameworkElement;

		private ArtboardAdorner Adorner => (ArtboardAdorner) FrameworkElement;

		private GeneralTransform CanvasTransform { get; set; }

		private GeneralTransform InversedCanvasTransform { get; set; }

		public override ResizeInfo ResizeInfo => new ResizeInfo(ConvertLocation(ActualHandle.OriginLocation), ConvertLocation(ActualHandle.CurrentLocation), ActualHandle.HandleKind);

		private Point ScrollPanelOrigin { get; set; }

		internal bool CanResizeStart()
		{
			var dragStarting = ResizeStarting;

			if (dragStarting == null)
				return true;

			var dragStartingEventArgs = new CancelEventArgs();

			dragStarting(this, dragStartingEventArgs);

			return dragStartingEventArgs.Cancel == false;
		}

		private Point ConvertLocation(Point location)
		{
			return Adorner.AdornerPanel.TransformToDesignCoordinates(location);
		}

		protected override IResizableAdvisor GetActualAdvisor()
		{
			var element = ActualElement;
			var visualParent = element?.GetVisualParent();

			if (visualParent is ArtboardCanvas canvas)
				return GetAdvisor(canvas);

			if (visualParent is Border)
				return ArtboardBorderResizableAdvisor.Instance;

			return null;
		}

		internal void OnMatrixChanged()
		{
			if (ActualHandle is ArtboardResizableElementHandleBase artboardDraggableElementHandle)
				artboardDraggableElementHandle.UpdateOriginLocation(Adorner.AdornerPanel.TransformFromDesignCoordinates(ScrollPanelOrigin));

			UpdatePosition();
		}

		protected override void OnResizeEnded()
		{
			base.OnResizeEnded();

			_snapEngineContext = _snapEngineContext.DisposeExchange();
		}

		protected override void OnResizeStarted()
		{
			base.OnResizeStarted();

			var snapSide = ArtboardSnapEngineUtils.GetResizeSide(ResizeInfo.HandleKind);
			var canvas = Adorner.ArtboardCanvas;
			var element = ActualElement;

			CanvasTransform = ((UIElement) element.GetVisualParent()).TransformToAncestor(canvas);
			InversedCanvasTransform = CanvasTransform.Inverse;

			_snapEngineContext = canvas.ArtboardControl?.SnapEngine?.CreateContext(new ArtboardSnapEngineContextParameters(ActualElement, snapSide));

			ScrollPanelOrigin = Adorner.AdornerPanel.TransformToDesignCoordinates(ActualHandle.OriginLocation);
		}

		protected override void SetPosition(Rect rect)
		{
			if (_snapEngineContext != null)
			{
				var elementRect = rect;
				var canvasRect = CanvasTransform.TransformBounds(elementRect);
				var snapCanvasRect = ArtboardSnapEngineUtils.CalcResizeRect(canvasRect, _snapEngineContext.Engine.Snap(new ArtboardSnapParameters(canvasRect, _snapEngineContext)).SnapRect, _snapEngineContext.Parameters.Side);
				var snapElementRect = InversedCanvasTransform.TransformBounds(snapCanvasRect);

				rect = snapElementRect;
			}

			base.SetPosition(rect);
		}
	}
}