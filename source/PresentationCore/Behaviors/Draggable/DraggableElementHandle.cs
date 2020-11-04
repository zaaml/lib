// <copyright file="DraggableHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal sealed class DraggableElementHandle : DraggableHandle
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<FrameworkElement, DraggableElementHandle>
			("Element", default, d => d.OnElementPropertyChangedPrivate);

		private IDraggableMouseInteraction _draggableMouse;

		public override Point CurrentLocation => DraggableMouse?.ScreenPoint ?? new Point();

		private IDraggableMouseInteraction DraggableMouse
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
			}
		}

		public FrameworkElement Element
		{
			get => (FrameworkElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		public override Point OriginLocation => DraggableMouse?.ScreenOrigin ?? new Point();

		private void AttachDraggableMouse(IDraggableMouseInteraction draggableMouse)
		{
			draggableMouse.DragStarted += OnDragStarted;
			draggableMouse.DragEnded += OnDragEnded;
			draggableMouse.DragMove += OnDragMove;
		}

		public override void BeginDrag()
		{
			DraggableMouse?.BeginDrag();
		}

		private void DetachDraggableMouse(IDraggableMouseInteraction draggableMouse)
		{
			draggableMouse.DragStarted -= OnDragStarted;
			draggableMouse.DragEnded -= OnDragEnded;
			draggableMouse.DragMove -= OnDragMove;
		}

		private void OnDragEnded(object sender, EventArgs e)
		{
			OnDragEnded();
		}

		private void OnDragMove(object sender, EventArgs e)
		{
			OnDragMove();
		}

		private void OnDragStarted(object sender, EventArgs e)
		{
			OnDragStarted();
		}

		private void OnElementPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			DraggableMouse = DraggableMouse.DisposeExchange(newValue != null ? DraggableMouseInteractionService.Create(newValue, 1000) : null);
		}

		public override void StopDrag()
		{
			DraggableMouse?.StopDrag();
		}
	}
}