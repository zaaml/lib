// <copyright file="ArtboardResizableElementHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	internal abstract class ArtboardResizableElementHandleBase : ResizableHandleBase
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<FrameworkElement, ArtboardResizableElementHandleBase>
			("Element", default, d => d.OnElementPropertyChangedPrivate);

		private Point _currentLocation;
		private ElementDragHandle _dragHandle;
		private Point _originLocation;

		protected ArtboardResizableElementHandleBase(ArtboardAdorner adorner)
		{
			Adorner = adorner;
		}

		public ArtboardAdorner Adorner { get; }

		private ArtboardAdornerPanel AdornerPanel { get; set; }

		public ArtboardResizableBehavior ArtboardBehavior => (ArtboardResizableBehavior) Behavior;

		protected bool CanResize => ArtboardBehavior?.CanResizeStart() ?? false;

		public override Point CurrentLocation => _currentLocation;

		public FrameworkElement Element
		{
			get => (FrameworkElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		protected override ResizableHandleKind HandleKindCore { get; set; }

		protected bool IsDragging => _dragHandle?.IsDragging ?? false;

		public override Point OriginLocation => _originLocation;

		private Point GetLocation(MouseEventArgs eventArgs)
		{
			return eventArgs.GetPosition(AdornerPanel);
		}

		private bool OnCanDragStart(MouseEventArgs arg)
		{
			return HandleKindCore != ResizableHandleKind.Undefined && Adorner.AdornerPanel != null && CanResize;
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

		private protected virtual void OnElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
		{
		}

		private void OnElementPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			_dragHandle = _dragHandle.DisposeExchange(newValue != null ? new ElementDragHandle(newValue, OnCanDragStart, OnDragStarted, OnDragEnded, OnDragMove) : null);

			OnElementChanged(oldValue, newValue);
		}

		internal void UpdateOriginLocation(Point location)
		{
			_originLocation = location;
		}
	}
}