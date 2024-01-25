// <copyright file="DraggableMouseInteractionService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Input
{
  internal class DraggableMouseInteractionService : ServiceBase<UIElement>
  {
    #region Fields

    private readonly List<DraggableMouseInteraction> _draggableMouseCollection = new List<DraggableMouseInteraction>();

    #endregion

    #region  Methods

    private void AddDraggableMouseInteraction(DraggableMouseInteraction draggableMouse)
    {
      _draggableMouseCollection.Add(draggableMouse);
    }

    public static IDraggableMouseInteraction Create(UIElement handle, int handlerPriority = 0)
    {
      var draggableMouseInteraction = new DraggableMouseInteraction(handle, handlerPriority);

      handle.GetServiceOrCreate(() => new DraggableMouseInteractionService()).AddDraggableMouseInteraction(draggableMouseInteraction);

      return draggableMouseInteraction;
    }

    private static DraggableMouseInteraction GetMouseInteraction(MouseEventArgs e)
    {
      var uie = PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource);

      return uie?.GetVisualAncestorsAndSelf()
        .OfType<UIElement>()
        .Select(u => u.GetService<DraggableMouseInteractionService>())
        .SkipNull()
        .SelectMany(s => s._draggableMouseCollection)
        .Where(d => d.WillHandleEvent(e))
        .LastMaxElementOrDefault(d => d.HandlerPriority);
    }

    protected override void OnAttach()
    {
      base.OnAttach();

      Target.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) UIElementOnMouseLeftButtonDown, true);
    }

    protected override void OnDetach()
    {
      Target.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) UIElementOnMouseLeftButtonDown);
      
      base.OnDetach();
    }

    private void RemoveDraggableMouseInteraction(DraggableMouseInteraction draggableMouse)
    {
      if (draggableMouse.IsDragging)
        draggableMouse.StopDrag();

      _draggableMouseCollection.Remove(draggableMouse);

      if (_draggableMouseCollection.Count == 0)
        Target.RemoveService<DraggableMouseInteractionService>();
    }

    private void UIElementOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
			if (e.ClickCount != 1)
				return;

      GetMouseInteraction(e)?.BeginDragImpl(e);
    }

    #endregion

    #region  Nested Types

    private sealed class DraggableMouseInteraction : IDraggableMouseInteraction
    {
      #region Fields

      private readonly UIElement _handle;
      private IDisposable _capture;
      private IDisposable _disposer;
      private bool _isEnabled = true;
      private bool _thresholdExceeded;

      #endregion

      #region Ctors

      public DraggableMouseInteraction(UIElement handle, int handlerPriority = 0)
      {
        _handle = handle;

        HandlerPriority = handlerPriority;
			}

      #endregion

      #region Properties

      private double DragLength => PointUtils.SubtractPoints(MouseInternal.ScreenLogicalPosition, ScreenOrigin).Length;

      public int HandlerPriority { get; }

      #endregion

      #region  Methods

      internal void BeginDragImpl(MouseButtonEventArgs e)
      {
        if (IsEnabled == false)
          return;

        if (IsDragging == false)
          StartDragging(e);
      }

      private void FinalizeDragging()
      {
        IsDragging = false;

        if (_thresholdExceeded)
          OnDragEnded();

        _capture = _capture.DisposeExchange();
      }

      private void OnDragEnded()
      {
        DragEnded?.Invoke(this, EventArgs.Empty);
      }

      private void OnDragMove()
      {
        DragMove?.Invoke(this, EventArgs.Empty);
      }

      private void OnDragStarted()
      {
        DragStarted?.Invoke(this, EventArgs.Empty);
      }

      private void OnDragStartedPrivate(MouseButtonEventArgs e)
      {
        IsDragging = true;

        ScreenOrigin = MouseInternal.ScreenLogicalPosition;
        ScreenPoint = ScreenOrigin;

        _thresholdExceeded = false;

        if (!Threshold.IsZero(2))
          return;

        _thresholdExceeded = true;

        OnDragStarted();
      }

      private void OnLostMouseCapture()
      {
        if (IsDragging)
          FinalizeDragging();
      }

      private void OnMouseLeftButtonUp(MouseButtonEventArgs e)
      {
        if (_capture == null || SkipEventHandling(e))
          return;

        try
        {
          FinalizeDragging();
        }
        finally
        {
          PostButtonEventHandling(e);
        }
      }

      private void OnMouseMove(MouseEventArgs e)
      {
        if (IsDragging == false || SkipEventHandling(e))
          return;

        try
        {
          ScreenPoint = MouseInternal.ScreenLogicalPosition;

          if (_thresholdExceeded)
            OnDragMove();
          else
          {
            if (DragLength.IsGreaterThanOrClose(Threshold) == false) 
	            return;

            _thresholdExceeded = true;

            OnDragStarted();
          }
        }
        finally
        {
          PostEventHandling(e);
        }
      }

      private void PostButtonEventHandling(MouseButtonEventArgs e)
      {
        if (MarkHandledEvents)
          e.Handled = true;
      }

      private void PostEventHandling([UsedImplicitly] MouseEventArgs e)
      {
#if !SILVERLIGHT
        if (MarkHandledEvents)
          e.Handled = true;
#endif
      }

      private bool SkipEventHandling(MouseEventArgs e)
      {
	      return ProcessHandledEvents == false && e is MouseButtonEventArgs md && md.Handled;
      }

      private void StartDragging(MouseButtonEventArgs e)
      {
        if (e != null && WillHandleEvent(e) == false)
          return;

        try
        {
          var controlHandle = _handle as Control;
          var cursor = controlHandle?.Cursor;

          _capture = DragProxy.Instance.CaptureMouse(OnMouseMove, OnMouseLeftButtonUp, OnLostMouseCapture, cursor);
          
          if (_capture != null)
            OnDragStartedPrivate(e);
        }
        finally
        {
          if (e != null)
            PostButtonEventHandling(e);
        }
      }

      internal bool WillHandleEvent(MouseEventArgs e)
      {
        return SkipEventHandling(e) == false;
      }

      #endregion

      #region Interface Implementations

      #region IDisposable

      public void Dispose()
      {
        _capture = _capture.DisposeExchange();
        _disposer = _disposer.DisposeExchange();

        _handle.GetService<DraggableMouseInteractionService>().RemoveDraggableMouseInteraction(this);
      }

      #endregion

      #region IDraggableMouseInteraction

      public event EventHandler DragEnded;
      public event EventHandler DragMove;
      public event EventHandler DragStarted;

      public Point ScreenOrigin { get; private set; }

      public Point ScreenPoint { get; private set; }

      public bool IsDragging { get; private set; }

      public bool MarkHandledEvents { get; set; }

      public bool ProcessHandledEvents { get; set; }

      public double Threshold { get; set; }

      public bool IsEnabled
      {
        get => _isEnabled;
        set
        {
          if (_isEnabled && value == false && IsDragging)
            StopDrag();

          _isEnabled = value;
        }
      }

      public void BeginDrag()
      {
        BeginDragImpl(null);
      }

      public void StopDrag()
      {
        if (IsDragging)
          FinalizeDragging();
      }

      #endregion

      #endregion
    }

    #endregion
  }
}