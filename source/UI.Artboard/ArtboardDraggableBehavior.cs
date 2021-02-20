// <copyright file="ArtboardDraggableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardDraggableBehavior : DraggableBehavior
	{
		internal event EventHandler<CancelEventArgs> DragStarting;

		protected override FrameworkElement ActualElement => Adorner?.AdornedElement ?? FrameworkElement;

		public ArtboardAdorner Adorner => (ArtboardAdorner) FrameworkElement;

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

		protected override IDraggableAdvisor GetActualAdvisor()
		{
			var element = ActualElement;
			var visualParent = element?.GetVisualParent();

			if (visualParent is ArtboardCanvas canvas)
				return GetAdvisor(canvas);

			if (visualParent is Border)
				return ArtboardMarginDraggableAdvisor.Instance;

			return null;
		}

		protected override void OnDragStarted()
		{
			base.OnDragStarted();

			ScrollPanelOrigin = Adorner.AdornerPanel.TransformToDesignCoordinates(ActualHandle.OriginLocation);
		}

		internal void OnMatrixChanged()
		{
			if (ActualHandle is ArtboardDraggableElementHandle artboardDraggableElementHandle)
				artboardDraggableElementHandle.UpdateOriginLocation(Adorner.AdornerPanel.TransformFromDesignCoordinates(ScrollPanelOrigin));

			UpdatePosition();
		}
	}

	internal sealed class ArtboardMarginDraggableAdvisor : ArtboardDraggableAdvisorBase
	{
		private ArtboardMarginDraggableAdvisor()
		{
		}

		public static ArtboardMarginDraggableAdvisor Instance { get; } = new ArtboardMarginDraggableAdvisor();

		protected override Point GetPositionCore(UIElement element)
		{
			if (element is FrameworkElement fre)
				return new Point(fre.Margin.Left, fre.Margin.Top);

			return new Point();
		}

		protected override void SetPositionCore(UIElement element, Point value)
		{
			if (element is FrameworkElement fre)
				fre.Margin = new Thickness(value.X, value.Y, fre.Margin.Right, fre.Margin.Bottom);
		}
	}
}