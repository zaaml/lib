// <copyright file="ArtboardDraggableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Behaviors.Draggable;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardDraggableBehavior : DraggableBehavior
	{
		internal event EventHandler<CancelEventArgs> DragStarting;

		protected override FrameworkElement ActualElement => Adorner?.AdornedElement as FrameworkElement ?? FrameworkElement;

		private ArtboardAdorner Adorner => (ArtboardAdorner) FrameworkElement;

		public override DragInfo DragInfo => new DragInfo(ConvertLocation(ActualHandle.OriginLocation), ConvertLocation(ActualHandle.CurrentLocation));

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

		internal void OnMatrixChanged()
		{
			if (ActualHandle is ArtboardDraggableElementHandle artboardDraggableElementHandle)
				artboardDraggableElementHandle.UpdateOriginLocation(Adorner.AdornerPanel.TransformFromDesignCoordinates(ScrollPanelOrigin));

			UpdatePosition();
		}

		protected override void OnDragStarted()
		{
			base.OnDragStarted();

			ScrollPanelOrigin = Adorner.AdornerPanel.TransformToDesignCoordinates(ActualHandle.OriginLocation);
		}
	}
}