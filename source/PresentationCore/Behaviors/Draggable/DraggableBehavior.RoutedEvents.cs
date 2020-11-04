// <copyright file="DraggableBehavior.RoutedEvents.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal partial class DraggableBehavior
	{
		public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent
			("DragStarted", RoutingStrategy.Bubble, typeof(DragStartedRoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent DragEnterEvent = EventManager.RegisterRoutedEvent
			("DragEnter", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent DragOverEvent = EventManager.RegisterRoutedEvent
			("DragOver", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent PreviewDragOverEvent = EventManager.RegisterRoutedEvent
			("PreviewDragOver", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent DragLeaveEvent = EventManager.RegisterRoutedEvent
			("DragLeave", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent DragDropEvent = EventManager.RegisterRoutedEvent
			("DragDrop", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent PreviewDragStartedEvent = EventManager.RegisterRoutedEvent
			("PreviewDragStarted", RoutingStrategy.Tunnel, typeof(DragStartedRoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent DragEndedEvent = EventManager.RegisterRoutedEvent
			("DragEnded", RoutingStrategy.Bubble, typeof(DragEndedRoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent PreviewDragEndedEvent = EventManager.RegisterRoutedEvent
			("PreviewDragEnded", RoutingStrategy.Tunnel, typeof(DragEndedRoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent DragMoveEvent = EventManager.RegisterRoutedEvent
			("DragMove", RoutingStrategy.Bubble, typeof(DragMoveRoutedEventHandler), typeof(DraggableBehavior));

		public static readonly RoutedEvent PreviewDragMoveEvent = EventManager.RegisterRoutedEvent
			("PreviewDragMove", RoutingStrategy.Tunnel, typeof(DragMoveRoutedEventHandler), typeof(DraggableBehavior));

		public static void AddDragDropHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(DragDropEvent, handler);
		}

		public static void AddDragEndedHandler(FrameworkElement frameworkElement, DragEndedRoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(DragEndedEvent, handler);
		}

		public static void AddDragEnterHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			AddDragEnterOverLeaveHandler(frameworkElement, handler);

			frameworkElement.AddRoutedHandler(DragEnterEvent, handler);
		}

		private static void AddDragEnterOverLeaveHandler(FrameworkElement frameworkElement, object handler)
		{
			frameworkElement.GetServiceOrCreate(() => new DragEnterOverLeaveService()).AddHandler(handler);
		}

		public static void AddDragLeaveHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			AddDragEnterOverLeaveHandler(frameworkElement, handler);

			frameworkElement.AddRoutedHandler(DragLeaveEvent, handler);
		}

		public static void AddDragMoveHandler(FrameworkElement frameworkElement, DragMoveRoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(DragMoveEvent, handler);
		}

		public static void AddDragOverHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(DragOverEvent, handler);
		}

		public static void AddDragStartedHandler(FrameworkElement frameworkElement, DragStartedRoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(DragStartedEvent, handler);
		}

		public static void AddPreviewDragEndedHandler(FrameworkElement frameworkElement, DragEndedRoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(PreviewDragEndedEvent, handler);
		}

		public static void AddPreviewDragMoveHandler(FrameworkElement frameworkElement, DragMoveRoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(PreviewDragMoveEvent, handler);
		}

		public static void AddPreviewDragOverHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(PreviewDragOverEvent, handler);
		}

		public static void AddPreviewDragStartedHandler(FrameworkElement frameworkElement, DragStartedRoutedEventHandler handler)
		{
			frameworkElement.AddRoutedHandler(PreviewDragStartedEvent, handler);
		}

		private static void RaiseDragDrop(FrameworkElement frameworkElement)
		{
			if (SkipEvents(frameworkElement))
				return;

			if (SkipDragDropEvent(frameworkElement))
				return;

			frameworkElement.RaiseRoutedEvent(new RoutedEventArgs(DragDropEvent, frameworkElement));
		}

		private static void RaiseDragEnter(FrameworkElement frameworkElement)
		{
			if (SkipEvents(frameworkElement))
				return;

			if (SkipDragDropEvent(frameworkElement))
				return;

			frameworkElement.RaiseRoutedEvent(new RoutedEventArgs(DragEnterEvent));
		}

		private static void RaiseDragLeave(FrameworkElement frameworkElement)
		{
			if (SkipEvents(frameworkElement))
				return;

			if (SkipDragDropEvent(frameworkElement))
				return;

			frameworkElement.RaiseRoutedEvent(new RoutedEventArgs(DragLeaveEvent));
		}

		private static void RaiseDragOver(FrameworkElement frameworkElement)
		{
			if (SkipEvents(frameworkElement))
				return;

			if (SkipDragDropEvent(frameworkElement))
				return;

			var args = new RoutedEventArgs(PreviewDragOverEvent, frameworkElement);

			frameworkElement.RaiseRoutedEvent(args);

			if (args.Handled)
				return;

			frameworkElement.RaiseRoutedEvent(new RoutedEventArgs(DragOverEvent, frameworkElement));
		}

		private void RaiseRoutedDragEnd()
		{
			var frameworkElement = ActualElement;

			if (SkipEvents(frameworkElement))
				return;

			var args = new DragEndedRoutedEventArgs(PreviewDragEndedEvent, this);

			frameworkElement.RaiseRoutedEvent(args);

			if (args.Handled)
				return;

			frameworkElement.RaiseRoutedEvent(new DragEndedRoutedEventArgs(DragEndedEvent, this));
		}

		private void RaiseRoutedDragMove()
		{
			var frameworkElement = ActualElement;

			if (SkipEvents(frameworkElement))
				return;

			var args = new DragMoveRoutedEventArgs(PreviewDragMoveEvent, this);

			frameworkElement.RaiseRoutedEvent(args);

			if (args.Handled)
				return;

			frameworkElement.RaiseRoutedEvent(new DragMoveRoutedEventArgs(DragMoveEvent, this));
		}

		private void RaiseRoutedDragStart()
		{
			var frameworkElement = ActualElement;

			if (SkipEvents(frameworkElement))
				return;

			var args = new DragStartedRoutedEventArgs(PreviewDragStartedEvent, this);

			frameworkElement.RaiseRoutedEvent(args);

			if (args.Handled)
				return;

			frameworkElement.RaiseRoutedEvent(new DragStartedRoutedEventArgs(DragStartedEvent, this));
		}

		public static void RemoveDragDropHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			AddDragEnterOverLeaveHandler(frameworkElement, handler);

			frameworkElement.RemoveRoutedHandler(DragDropEvent, handler);
		}

		public static void RemoveDragEndedHandler(FrameworkElement frameworkElement, DragEndedRoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(DragEndedEvent, handler);
		}

		public static void RemoveDragEnterHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			RemoveDragEnterOverLeaveHandler(frameworkElement, handler);

			frameworkElement.RemoveRoutedHandler(DragEnterEvent, handler);
		}

		private static void RemoveDragEnterOverLeaveHandler(FrameworkElement frameworkElement, object handler)
		{
			var svc = frameworkElement.GetService<DragEnterOverLeaveService>();

			if (svc == null)
				return;

			svc.RemoveHandler(handler);

			if (svc.HandlersCount == 0)
				frameworkElement.RemoveService<DragEnterOverLeaveService>();
		}

		public static void RemoveDragLeaveHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			RemoveDragEnterOverLeaveHandler(frameworkElement, handler);

			frameworkElement.RemoveRoutedHandler(DragLeaveEvent, handler);
		}

		public static void RemoveDragMoveHandler(FrameworkElement frameworkElement, DragMoveRoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(DragMoveEvent, handler);
		}

		public static void RemoveDragOverHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			RemoveDragEnterOverLeaveHandler(frameworkElement, handler);

			frameworkElement.RemoveRoutedHandler(DragOverEvent, handler);
		}

		public static void RemoveDragStartedHandler(FrameworkElement frameworkElement, DragStartedRoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(DragStartedEvent, handler);
		}

		public static void RemovePreviewDragEndedHandler(FrameworkElement frameworkElement, DragEndedRoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(PreviewDragEndedEvent, handler);
		}

		public static void RemovePreviewDragMoveHandler(FrameworkElement frameworkElement, DragMoveRoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(PreviewDragMoveEvent, handler);
		}

		public static void RemovePreviewDragOverHandler(FrameworkElement frameworkElement, RoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(PreviewDragOverEvent, handler);
		}

		public static void RemovePreviewDragStartedHandler(FrameworkElement frameworkElement, DragStartedRoutedEventHandler handler)
		{
			frameworkElement.RemoveRoutedHandler(PreviewDragStartedEvent, handler);
		}

		private static bool SkipDragDropEvent(FrameworkElement frameworkElement)
		{
			return false;
		}

		private static bool SkipEvents(FrameworkElement frameworkElement)
		{
			return frameworkElement == null;
		}

		private sealed class DragEnterOverLeaveService : ServiceBase<FrameworkElement>
		{
			private readonly List<object> _handlers = new List<object>();
			private bool _isMouseOver;
			private Rect? _layoutBox;

			public int HandlersCount => _handlers.Count;

			private bool IsMouseOver
			{
				get => _isMouseOver;
				set
				{
					if (_isMouseOver == value)
						return;

					if (value)
						RaiseDragEnter(Target);
					else
						RaiseDragLeave(Target);

					_isMouseOver = value;
				}
			}

			private Rect LayoutBox => _layoutBox ??= Target.GetScreenBox();

			public void AddHandler(object handler)
			{
				_handlers.Add(handler);
			}

			protected override void OnAttach()
			{
				base.OnAttach();

				MouseInternal.MouseMove += OnGlobalMouseMove;
				GlobalDragEnding += OnGlobalDragEnding;
				Target.LayoutUpdated += TargetOnLayoutUpdated;
				ProcessMouseEventSource();
			}

			protected override void OnDetach()
			{
				Target.LayoutUpdated -= TargetOnLayoutUpdated;
				MouseInternal.MouseMove -= OnGlobalMouseMove;
				GlobalDragEnding -= OnGlobalDragEnding;

				base.OnDetach();
			}

			private void OnGlobalDragEnding(object sender, EventArgs eventArgs)
			{
				if (IsMouseOver)
					RaiseDragDrop(Target);
			}

			private void OnGlobalMouseMove(object sender, MouseEventArgsInt mouseEventArgsInt)
			{
				ProcessMouseEventSource();

				if (IsMouseOver)
					RaiseDragOver(Target);
			}

			private void ProcessMouseEventSource()
			{
				if (IsDragging)
					IsMouseOver = LayoutBox.Contains(MouseInternal.ScreenPosition);
			}

			public void RemoveHandler(object handler)
			{
				_handlers.Remove(handler);
			}

			private void TargetOnLayoutUpdated(object sender, EventArgs eventArgs)
			{
				_layoutBox = null;
			}
		}
	}
}