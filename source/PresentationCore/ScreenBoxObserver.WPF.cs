// <copyright file="ScreenBoxObserver.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore
{
  internal partial class ScreenBoxObserver
  {
    #region  Methods

    partial void PlatformCtor()
    {
      WindowLocationObserver.RegisterObserver(this);
    }

    #endregion

    #region  Nested Types

    private class WindowLocationObserver : ServiceBase<Window>
    {
      #region Static Fields and Constants

      private static readonly WeakLinkedList<ScreenBoxObserver> Observers = new WeakLinkedList<ScreenBoxObserver>();
      private static bool _isObservingWindows;

      #endregion

      #region Properties

      private static bool IsObservingWindows
      {
        get => _isObservingWindows;
        set
        {
          if (_isObservingWindows == value)
            return;

          _isObservingWindows = value;

          if (_isObservingWindows)
            CompositionTarget.Rendering += CompositionTargetOnRendering;
          else
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }
      }

      #endregion

      #region  Methods

      private static void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
      {
        foreach (var window in PresentationTreeUtils.EnumerateVisualRoots().OfType<Window>())
          EnsureService(window);

        IsObservingWindows = Observers.IsEmpty == false;
      }

      private static void EnsureService(Window window)
      {
        if (window.Dispatcher.CheckAccess())
          window.GetServiceOrCreate(() => new WindowLocationObserver());
        else
          window.Dispatcher.BeginInvoke(() => EnsureService(window));
      }

      protected override void OnAttach()
      {
        base.OnAttach();

        Target.LocationChanged += TargetOnLocationChanged;
        Target.Closed += TargetOnClosed;
      }

      protected override void OnDetach()
      {
        Target.Closed -= TargetOnClosed;
        Target.LocationChanged -= TargetOnLocationChanged;

        base.OnDetach();
      }

      public static void RegisterObserver(ScreenBoxObserver observer)
      {
        Observers.Add(observer);
        IsObservingWindows = true;
      }

      private void TargetOnClosed(object sender, EventArgs eventArgs)
      {
        Target.RemoveService<WindowLocationObserver>();
      }

      private void TargetOnLocationChanged(object sender, EventArgs eventArgs)
      {
        foreach (var observer in Observers.Where(o => o.IsDisposed == false))
        {
          if (ReferenceEquals(Window.GetWindow(observer.FrameworkElement), Target))
            observer.UpdateScreenBox();
        }
      }

      #endregion
    }

    #endregion
  }
}