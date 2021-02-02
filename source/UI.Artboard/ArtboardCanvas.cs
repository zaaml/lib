// <copyright file="ArtboardCanvas.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardCanvas : ArtboardPanel
	{
		public static readonly DependencyProperty XProperty = DPM.RegisterAttached<double, ArtboardCanvas>
			("X", 0.0, OnXPropertyChangedPrivate);

		public static readonly DependencyProperty YProperty = DPM.RegisterAttached<double, ArtboardCanvas>
			("Y", 0.0, OnYPropertyChangedPrivate);

		private static readonly DependencyProperty CanvasProperty = DPM.RegisterAttached<ArtboardCanvas, ArtboardCanvas>
			("Canvas", null, OnCanvasPropertyChangedPrivate);

		private readonly WeakLinkedList<UIElement> _canvasElements = new WeakLinkedList<UIElement>();
		private readonly Func<UIElement, bool> _childCleanupPredicate;

		public ArtboardCanvas()
		{
			_childCleanupPredicate = CleanChild;

			DraggableBehavior.SetAdvisor(this, new ArtboardCanvasDraggableAdvisor(this));
			ResizableBehavior.SetAdvisor(this, new ArtboardCanvasResizableAdvisor(this));
		}

		private IEnumerable<UIElement> Elements => Children.OfType<UIElement>();

		internal void ArrangeChild(UIElement child, bool forceMeasure = false)
		{
			if (forceMeasure)
				child.Measure(XamlConstants.InfiniteSize);

			var rect = new Rect(new Point(GetX(child), GetY(child)), child.DesiredSize);

			child.Arrange(rect);

			ArtboardControl?.ArrangeAdorners(child, rect);
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			foreach (UIElement child in Children)
				ArrangeChild(child);

			return finalSize;
		}

		private protected override void AttachArtboard(ArtboardControl artboard)
		{
			base.AttachArtboard(artboard);

			foreach (var element in Elements)
				AttachFactoryAdorners(element);
		}

		private void AttachElement(UIElement element)
		{
			AttachFactoryAdorners(element);

			UpdateAdorners(element);
		}

		private void AttachFactoryAdorners(UIElement element)
		{
			if (ArtboardControl == null)
				return;

			foreach (var adornerFactory in ArtboardControl.AdornerFactories)
				AttachFactoryAdorners(element, adornerFactory);
		}

		private static void AttachFactoryAdorners(UIElement element, ArtboardAdornerFactory adornerFactory)
		{
			ArtboardControl.GetAdorners(element).Add(adornerFactory.CreateAdorner());
		}

		private bool CleanChild(UIElement element)
		{
			var isChild = ReferenceEquals(element.GetVisualParent(), this);

			if (isChild)
				return false;

			if (ReferenceEquals(GetCanvas(element), this))
				SetCanvas(element, null);

			return true;
		}

		private protected override void DetachArtboard(ArtboardControl artboard)
		{
			foreach (var element in Elements)
				DetachFactoryAdorners(element);

			base.DetachArtboard(artboard);
		}

		private void DetachElement(UIElement element)
		{
			DetachFactoryAdorners(element);

			UpdateAdorners(element);
		}

		private void DetachFactoryAdorners(UIElement element)
		{
			if (ArtboardControl == null)
				return;

			foreach (var adornerFactory in ArtboardControl.AdornerFactories)
				DetachFactoryAdorners(element, adornerFactory);
		}

		private static void DetachFactoryAdorners(UIElement element, ArtboardAdornerFactory adornerFactory)
		{
			var adorners = ArtboardControl.GetAdornersInternal(element);

			if (adorners == null)
				return;

			for (var index = 0; index < adorners.Count; index++)
			{
				var adorner = adorners[index];

				if (ReferenceEquals(adorner.Factory, adornerFactory))
				{
					adorners.RemoveAt(index);
					index--;
				}
			}
		}

		private static ArtboardCanvas GetCanvas(UIElement element)
		{
			return (ArtboardCanvas) element.GetValue(CanvasProperty);
		}

		public static Point GetPosition(UIElement element)
		{
			return new Point(GetX(element), GetY(element));
		}

		public static double GetX(DependencyObject dependencyObject)
		{
			return (double) dependencyObject.GetValue(XProperty);
		}

		public static double GetY(DependencyObject dependencyObject)
		{
			return (double) dependencyObject.GetValue(YProperty);
		}

		private static void InvalidateCanvasArrange(DependencyObject dependencyObject)
		{
			if (dependencyObject is UIElement uie && uie.GetVisualParent() is ArtboardCanvas artboardCanvas)
				artboardCanvas.InvalidateArrange();
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			foreach (UIElement child in Children)
			{
				if (ReferenceEquals(this, GetCanvas(child)) == false)
				{
					_canvasElements.Add(child);

					SetCanvas(child, this);
				}

				child.Measure(XamlConstants.InfiniteSize);
			}

			_canvasElements.Cleanup(_childCleanupPredicate);

			return new Size(0, 0);
		}

		internal void OnAdornerFactoryAdded(ArtboardAdornerFactory adornerFactory)
		{
			foreach (var element in Elements)
				AttachFactoryAdorners(element, adornerFactory);
		}

		internal void OnAdornerFactoryRemoved(ArtboardAdornerFactory adornerFactory)
		{
			foreach (var element in Elements)
				DetachFactoryAdorners(element, adornerFactory);
		}

		private static void OnCanvasPropertyChangedPrivate(DependencyObject dependencyObject, ArtboardCanvas oldCanvas, ArtboardCanvas newCanvas)
		{
			if (ReferenceEquals(oldCanvas, newCanvas))
				return;

			oldCanvas?.DetachElement((UIElement) dependencyObject);
			newCanvas?.AttachElement((UIElement) dependencyObject);
		}

		private static void OnXPropertyChangedPrivate(DependencyObject dependencyObject, double oldValue, double newValue)
		{
			InvalidateCanvasArrange(dependencyObject);
		}

		private static void OnYPropertyChangedPrivate(DependencyObject dependencyObject, double oldValue, double newValue)
		{
			InvalidateCanvasArrange(dependencyObject);
		}

		private static void SetCanvas(UIElement element, ArtboardCanvas canvas)
		{
			element.SetValue(CanvasProperty, canvas);
		}

		public static void SetPosition(UIElement element, Point position)
		{
			SetX(element, position.X);
			SetY(element, position.Y);
		}

		public static void SetX(DependencyObject dependencyObject, double x)
		{
			dependencyObject.SetValue(XProperty, x);
		}

		public static void SetY(DependencyObject dependencyObject, double y)
		{
			dependencyObject.SetValue(YProperty, y);
		}

		private static void UpdateAdorners(UIElement element)
		{
			var adorners = ArtboardControl.GetAdornersInternal(element);

			if (adorners == null)
				return;

			foreach (var adorner in adorners)
				adorner.UpdatePanel();
		}
	}
}