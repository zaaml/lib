// <copyright file="DragProxy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.Core.Pools;

namespace Zaaml.PresentationCore.Input
{
  internal sealed class DragProxy
  {
    #region Static Fields and Constants

    private static readonly Lazy<DragProxy> LazyInstance = new Lazy<DragProxy>(() => new DragProxy());

    #endregion

    #region Fields

    private readonly LightObjectPool<Canvas> _canvasPool;
    private readonly Canvas _globalCanvas = new Canvas();
    private readonly Popup _popup = new Popup();
    private IDisposable _currentCapture;

    #endregion

    #region Ctors

    private DragProxy()
    {
      _canvasPool = new LightObjectPool<Canvas>(() => new Canvas(), InitCanvas, ReleaseCanvas);
      _popup.Child = _globalCanvas;
    }

    #endregion

    #region Properties

    public static DragProxy Instance => LazyInstance.Value;

    #endregion

    #region  Methods

    public IDisposable CaptureMouse(Action<MouseEventArgs> onMouseMove, Action<MouseButtonEventArgs> onMouseLeftButtonUp, Action onLostMouseCapture, Cursor cursor = null)
    {
      _currentCapture = _currentCapture?.DisposeExchange();

      var canvas = _canvasPool.GetObject();

      canvas.Cursor = cursor;

      var isCaptured = canvas.CaptureMouse();

      if (isCaptured == false) 
	      return null;

      var dragProxyImpl = new DragProxyImpl(canvas, onMouseMove, onMouseLeftButtonUp, d =>
      {
        _canvasPool.Release(d.Canvas);

        if (ReferenceEquals(_currentCapture, d))
          onLostMouseCapture();
      }, OnDisposed);

      return _currentCapture = dragProxyImpl;
    }

    private void InitCanvas(Canvas obj)
    {
      _globalCanvas.Children.Add(obj);

      UpdatePopup();
    }

    private void OnDisposed(DragProxyImpl proxy)
    {
      if (ReferenceEquals(proxy, _currentCapture) == false)
        return;

      _currentCapture = null;

      proxy.Canvas.ReleaseMouseCapture();
    }

    private void ReleaseCanvas(Canvas obj)
    {
      _globalCanvas.Children.Remove(obj);

      UpdatePopup();
    }

    private void UpdatePopup()
    {
      _popup.IsOpen = _globalCanvas.Children.Count > 0;
    }

    #endregion

    #region  Nested Types

    private class DragProxyImpl : IDisposable
    {
      #region Fields

      private readonly Canvas _canvas;
      private readonly Action<DragProxyImpl> _onDisposed;
      private readonly Action<DragProxyImpl> _onLostMouseCapture;
      private readonly Action<MouseButtonEventArgs> _onMouseLeftButtonUp;
      private readonly Action<MouseEventArgs> _onMouseMove;
      private bool _isDisposed;

      #endregion

      #region Ctors

      public DragProxyImpl(Canvas canvas, Action<MouseEventArgs> onMouseMove, Action<MouseButtonEventArgs> onMouseLeftButtonUp,
        Action<DragProxyImpl> onLostMouseCapture, Action<DragProxyImpl> onDisposed)
      {
        _canvas = canvas;
        _onMouseMove = onMouseMove;
        _onMouseLeftButtonUp = onMouseLeftButtonUp;
        _onLostMouseCapture = onLostMouseCapture;
        _onDisposed = onDisposed;

        _canvas.MouseMove += OnMouseMove;
        _canvas.LostMouseCapture += OnLostMouseCapture;
        _canvas.MouseLeftButtonUp += OnMouseLeftButtonUp;
      }

      #endregion

      #region Properties

      public Canvas Canvas => _canvas;

      #endregion

      #region  Methods

      private void OnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
      {
        _canvas.LostMouseCapture -= OnLostMouseCapture;
        _onLostMouseCapture(this);

        Dispose();
      }

      private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
        _onMouseLeftButtonUp(e);
      }

      private void OnMouseMove(object sender, MouseEventArgs e)
      {
        _onMouseMove(e);
      }

      #endregion

      #region Interface Implementations

      #region IDisposable

      public void Dispose()
      {
        if (_isDisposed)
	        return;

        _canvas.MouseMove -= OnMouseMove;
        _canvas.MouseLeftButtonUp -= OnMouseLeftButtonUp;

        _isDisposed = true;
        _onDisposed(this);
      }

      #endregion

      #endregion
    }

    #endregion
  }
}