// <copyright file="ArtboardResizableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Behaviors.Resizable;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardResizableBehavior : ResizableBehavior
	{
		internal event EventHandler<CancelEventArgs> ResizeStarting;

		private protected override FrameworkElement ActualElement => Adorner?.AdornedElement as FrameworkElement ?? FrameworkElement;

		private ArtboardAdorner Adorner => (ArtboardAdorner) FrameworkElement;

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

		internal void OnMatrixChanged()
		{
			if (ActualHandle is ArtboardResizableElementHandleBase artboardDraggableElementHandle)
				artboardDraggableElementHandle.UpdateOriginLocation(Adorner.AdornerPanel.TransformFromDesignCoordinates(ScrollPanelOrigin));

			UpdatePosition();
		}

		protected override void OnResizeStarted()
		{
			base.OnResizeStarted();

			ScrollPanelOrigin = Adorner.AdornerPanel.TransformToDesignCoordinates(ActualHandle.OriginLocation);
		}
	}
}