// <copyright file="ScreenBoxObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
  internal partial class ScreenBoxObserver : IDisposable
  {
    #region Fields

    private readonly Action _notifyCallback;
    private Rect _screenBox;

    #endregion

    #region Ctors

    public ScreenBoxObserver(FrameworkElement frameworkElement, Action notifyCallback)
    {
      _screenBox = frameworkElement.GetScreenBox();
      _notifyCallback = notifyCallback;

      FrameworkElement = frameworkElement;
      FrameworkElement.LayoutUpdated += FrameworkElementOnLayoutUpdated;

      PlatformCtor();
    }

    #endregion

    #region Properties

    public FrameworkElement FrameworkElement { get; private set; }

    public bool IsDisposed => FrameworkElement == null;

    public Rect ScreenBox
    {
      get => _screenBox;
      private set
      {
        if (_screenBox.IsCloseTo(value))
          return;

        _screenBox = value;
        _notifyCallback();
      }
    }

    #endregion

    #region  Methods

    private void FrameworkElementOnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      UpdateScreenBox();
    }

    partial void PlatformCtor();

    private void UpdateScreenBox()
    {
      if (FrameworkElement == null)
        return;

      ScreenBox = FrameworkElement.GetScreenBox();
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      if (IsDisposed)
        return;

      FrameworkElement.LayoutUpdated -= FrameworkElementOnLayoutUpdated;
      FrameworkElement = null;
    }

    #endregion

    #endregion
  }
}