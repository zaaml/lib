// <copyright file="ResizableElementHandleBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal abstract class ResizableElementHandleBase : ResizableHandleBase
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<FrameworkElement, ResizableElementHandleBase>
			("Element", h => h.OnElementChanged);

		private IDraggableMouseInteraction _draggableMouse;

		protected IDraggableMouseInteraction DraggableMouse
		{
			get => _draggableMouse;
			set
			{
				if (ReferenceEquals(_draggableMouse, value))
					return;

				if (_draggableMouse != null)
					DetachDraggableMouse(_draggableMouse);

				_draggableMouse = value;

				if (_draggableMouse != null)
					AttachDraggableMouse(_draggableMouse);

				UpdateCursor();
			}
		}

		public FrameworkElement Element
		{
			get => (FrameworkElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		public sealed override Point OriginLocation => DraggableMouse?.ScreenOrigin ?? new Point();

		public sealed override Point CurrentLocation => DraggableMouse?.ScreenPoint ?? new Point();

		private void AttachDraggableMouse(IDraggableMouseInteraction draggableMouse)
		{
			draggableMouse.DragStarted += OnDragStarted;
			draggableMouse.DragEnded += OnDragEnded;
			draggableMouse.DragMove += OnDragMove;
		}

		private void DetachDraggableMouse(IDraggableMouseInteraction draggableMouse)
		{
			draggableMouse.DragStarted -= OnDragStarted;
			draggableMouse.DragEnded -= OnDragEnded;
			draggableMouse.DragMove -= OnDragMove;
		}

		private void OnDragEnded(object sender, EventArgs e)
		{
			if (HandleKindCore != ResizableHandleKind.Undefined)
				OnDragEnded();
		}

		private void OnDragMove(object sender, EventArgs e)
		{
			if (HandleKindCore != ResizableHandleKind.Undefined)
				OnDragMove();
		}

		private void OnDragStarted(object sender, EventArgs e)
		{
			if (HandleKindCore != ResizableHandleKind.Undefined)
				OnDragStarted();
		}

		private void OnElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.MouseMove -= OnElementMouseMove;

			if (newValue != null)
				newValue.MouseMove += OnElementMouseMove;

			DraggableMouse = DraggableMouse.DisposeExchange(newValue != null ? DraggableMouseInteractionService.Create(newValue, 3000) : null);
		}

		private protected virtual void OnElementMouseMove(object sender, MouseEventArgs e)
		{
		}

		protected override void UpdateCursor()
		{
			if (Element != null)
				ResizableBehavior.UpdateCursor(Element, HandleKindCore);
		}
	}
}