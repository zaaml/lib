// <copyright file="ArtboardResizableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardResizableBehavior : ResizableBehavior
	{
		internal event EventHandler<CancelEventArgs> ResizeStarting;

		private protected override FrameworkElement ActualElement => Adorner?.AdornedElement ?? FrameworkElement;

		public ArtboardAdorner Adorner => (ArtboardAdorner) FrameworkElement;

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
				return ArtboardMarginResizableAdvisor.Instance;

			return null;
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

	internal sealed class ArtboardMarginResizableAdvisor : ArtboardResizableAdvisorBase
	{
		private ArtboardMarginResizableAdvisor()
		{
		}

		public static ArtboardMarginResizableAdvisor Instance { get; } = new ArtboardMarginResizableAdvisor();

		protected override Rect GetBoundingBoxCore(UIElement element)
		{
			if (element is FrameworkElement fre)
				return new Rect(new Point(fre.Margin.Left, fre.Margin.Top), new Size(fre.Width, fre.Height));

			return Rect.Empty;
		}

		protected override void SetBoundingBoxCore(UIElement element, Rect rect)
		{
			if (element is FrameworkElement fre)
			{
				fre.Margin = new Thickness(rect.X, rect.Y, fre.Margin.Right, fre.Margin.Bottom);
				fre.Width = rect.Width;
				fre.Height = rect.Height;
			}
		}
	}
}