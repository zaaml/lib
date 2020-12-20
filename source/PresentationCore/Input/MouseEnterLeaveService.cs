// <copyright file="MouseEnterLeaveService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Input
{
  internal class MouseEnterLeaveService : ServiceBase<FrameworkElement>
  {
    #region Static Fields and Constants

    private static readonly FrameworkElement UnsetUIElement = new Fr();

    #endregion

    #region Fields

    private bool _isMouseOver;
    private WeakReference _lastMouseEventSource = new WeakReference(UnsetUIElement);
    private Rect? _layoutBox;

    public event EventHandler IsMouseOverChanged;

    public event EventHandler MouseEnter;

    public event EventHandler MouseLeave;

    #endregion

    #region Properties

    public bool IsMouseOver
    {
      get => _isMouseOver;
      private set
      {
        if (_isMouseOver == value)
          return;

        if (value)
          OnMouseEnter();
        else
          OnMouseLeave();

        _isMouseOver = value;
        OnIsMouseOverChanged();
      }
    }

    public Rect LayoutBox => (_layoutBox ?? (_layoutBox = Target.DirectCast<FrameworkElement>().GetScreenBox())).Value;

    #endregion

    #region  Methods

    protected override void OnAttach()
    {
      base.OnAttach();
      
      MouseInternal.MouseMove += OnGlobalMouseMove;
      Target.LayoutUpdated += TargetOnLayoutUpdated;
      
      ProcessMouseEventSource();
    }

    protected override void OnDetach()
    {
      Target.LayoutUpdated -= TargetOnLayoutUpdated;
      MouseInternal.MouseMove -= OnGlobalMouseMove;
      
      base.OnDetach();
    }

    private void OnGlobalMouseMove(object sender, MouseEventArgsInt mouseEventArgsInt)
    {
      ProcessMouseEventSource();
    }

    protected virtual void OnIsMouseOverChanged()
    {
      IsMouseOverChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnMouseEnter()
    {
      MouseEnter?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnMouseLeave()
    {
      MouseLeave?.Invoke(this, EventArgs.Empty);
    }

    private void ProcessMouseEventSource()
    {
      var fre = MouseInternal.DirectlyOver as FrameworkElement;
      
      if (fre == null)
        return;

      var lastSourceRef = _lastMouseEventSource.GetTarget<FrameworkElement>();
      if (ReferenceEquals(lastSourceRef, fre) && MouseInternal.IsMouseCaptured == false)
        return;

      IsMouseOver = MouseInternal.IsMouseCaptured == false ? fre.IsVisualDescendantOf(Target) : LayoutBox.Contains(MouseInternal.ScreenPosition);

      _lastMouseEventSource = new WeakReference(fre);
    }

    private void TargetOnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      _layoutBox = null;
    }

    #endregion

    #region  Nested Types

    private class Fr : FrameworkElement
    {
    }

    #endregion
  }
}