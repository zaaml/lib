// <copyright file="ArtboardDraggableElementHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardDraggableElementHandle : DraggableHandle
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<FrameworkElement, ArtboardDraggableElementHandle>
			("Element", default, d => d.OnElementPropertyChangedPrivate);

		private Point _currentLocation;
		private ElementDragHandle _dragHandle;
		private Point _originLocation;


		public ArtboardDraggableElementHandle(ArtboardAdorner adorner)
		{
			Adorner = adorner;
		}

		public ArtboardAdorner Adorner { get; }

		private ArtboardAdornerPanel AdornerPanel { get; set; }

		public ArtboardDraggableBehavior ArtboardBehavior => (ArtboardDraggableBehavior) Behavior;

		private bool CanDrag => ArtboardBehavior?.CanDragStart() ?? false;

		public override Point CurrentLocation => _currentLocation;

		public FrameworkElement Element
		{
			get => (FrameworkElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		public override Point OriginLocation => _originLocation;

		public override void BeginDrag()
		{
		}

		private Point GetLocation(MouseEventArgs eventArgs)
		{
			return eventArgs.GetPosition(AdornerPanel);
		}

		private bool OnCanDragStart(MouseEventArgs arg)
		{
			return Adorner.AdornerPanel != null && CanDrag;
		}

		private void OnDragEnded(MouseEventArgs eventArgs)
		{
			AdornerPanel = null;

			OnDragEnded();
		}

		private void OnDragMove(MouseEventArgs eventArgs)
		{
			_currentLocation = GetLocation(eventArgs);

			OnDragMove();
		}

		private void OnDragStarted(MouseEventArgs eventArgs)
		{
			AdornerPanel = Adorner.AdornerPanel;

			_originLocation = _currentLocation = GetLocation(eventArgs);

			OnDragStarted();
		}

		private void OnElementPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			_dragHandle = _dragHandle.DisposeExchange(newValue != null ? new ElementDragHandle(newValue, OnCanDragStart, OnDragStarted, OnDragEnded, OnDragMove) : null);
		}

		public override void StopDrag()
		{
		}

		internal void UpdateOriginLocation(Point location)
		{
			_originLocation = location;
		}
	}
}