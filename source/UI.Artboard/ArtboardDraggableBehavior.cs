// <copyright file="ArtboardDraggableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardDraggableBehavior : DraggableBehavior
	{
		private ArtboardSnapEngineContext _snapEngineContext;
		internal event EventHandler<CancelEventArgs> DragStarting;

		protected override FrameworkElement ActualElement => Adorner?.AdornedElement ?? FrameworkElement;

		private ArtboardAdorner Adorner => (ArtboardAdorner) FrameworkElement;

		private GeneralTransform CanvasTransform { get; set; }

		public override DragInfo DragInfo => new DragInfo(ConvertLocation(ActualHandle.OriginLocation), ConvertLocation(ActualHandle.CurrentLocation));

		private GeneralTransform InversedCanvasTransform { get; set; }

		private Point ScrollPanelOrigin { get; set; }

		public bool CanDragStart()
		{
			var dragStarting = DragStarting;

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

		protected override IDraggableAdvisor GetActualAdvisor()
		{
			var element = ActualElement;
			var visualParent = element?.GetVisualParent();

			if (visualParent is ArtboardCanvas canvas)
				return GetAdvisor(canvas);

			if (visualParent is Border)
				return ArtboardBorderDraggableAdvisor.Instance;

			return null;
		}

		protected override void OnDragEnded()
		{
			base.OnDragEnded();

			_snapEngineContext = _snapEngineContext.DisposeExchange();
		}

		protected override void OnDragStarted()
		{
			base.OnDragStarted();

			var canvas = Adorner.ArtboardCanvas;
			var element = ActualElement;

			CanvasTransform = ((UIElement) element.GetVisualParent()).TransformToAncestor(canvas);
			InversedCanvasTransform = CanvasTransform.Inverse;

			_snapEngineContext = canvas.ArtboardControl?.SnapEngine?.CreateContext(new ArtboardSnapEngineContextParameters(element, ArtboardSnapRectSide.All));

			ScrollPanelOrigin = Adorner.AdornerPanel.TransformToDesignCoordinates(ActualHandle.OriginLocation);
		}

		internal void OnMatrixChanged()
		{
			if (ActualHandle is ArtboardDraggableElementHandle artboardDraggableElementHandle)
				artboardDraggableElementHandle.UpdateOriginLocation(Adorner.AdornerPanel.TransformFromDesignCoordinates(ScrollPanelOrigin));

			UpdatePosition();
		}

		protected override void SetPosition(Point position)
		{
			if (_snapEngineContext != null)
			{
				var elementRect = new Rect(position, ActualElement.RenderSize);
				var canvasRect = CanvasTransform.TransformBounds(elementRect);
				var snapCanvasRect = _snapEngineContext.Engine.Snap(new ArtboardSnapParameters(canvasRect, _snapEngineContext)).SnapRect;
				var snapElementRect = InversedCanvasTransform.TransformBounds(snapCanvasRect);

				position = snapElementRect.GetTopLeft();
			}

			base.SetPosition(position);
		}
	}
}