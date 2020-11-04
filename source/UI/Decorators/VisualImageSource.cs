// <copyright file="VisualImageSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Decorators
{
  internal sealed class VisualImageSource : AssetBase
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ImageSourcePropertyKey = DPM.RegisterReadOnly<ImageSource, VisualImageSource>
      ("ImageSource");

    public static readonly DependencyProperty VisualProperty = DPM.Register<FrameworkElement, VisualImageSource>
      ("Visual", v => v.OnVisualChanged);

    public static readonly DependencyProperty ImageSourceProperty = ImageSourcePropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private WriteableBitmap _bufferBitmap;

#if SILVERLIGHT
    private WriteableBitmap _writeableBitmap;
#else
    private RenderTargetBitmap _writeableBitmap;
    private int[] _pixels;
    private int[] _bufferPixels;
#endif

    #endregion

    #region Properties

    internal WriteableBitmap BufferBitmap => _bufferBitmap;

    public FrameworkElement Visual
    {
      get => (FrameworkElement) GetValue(VisualProperty);
      set => SetValue(VisualProperty, value);
    }

    public ImageSource ImageSource
    {
      get => (ImageSource) GetValue(ImageSourceProperty);
      private set => this.SetReadOnlyValue(ImageSourcePropertyKey, value);
    }

    #endregion

    #region  Methods

    private void OnVisualChanged(FrameworkElement oldElement, FrameworkElement newElement)
    {
      if (oldElement != null)
        oldElement.LayoutUpdated -= SourceOnLayoutUpdated;

      if (newElement != null)
        newElement.LayoutUpdated += SourceOnLayoutUpdated;

      Update();
    }


    private void SourceOnLayoutUpdated(object o, EventArgs eventArgs)
    {
      Update();
    }

    private void Update()
    {
      UpdateBitmap();
      RenderBitmap();
    }

    //internal event EventHandler BufferChanged;

    private void RenderBitmap()
    {
      if (Visual == null || _writeableBitmap == null)
        return;

#if SILVERLIGHT
      Array.Clear(_writeableBitmap.Pixels, 0, _writeableBitmap.Pixels.Length);
      _writeableBitmap.Render(Visual, Visual.RenderTransform);
      _writeableBitmap.Invalidate();

      if (_writeableBitmap.Pixels.SequenceEqual(_bufferBitmap.Pixels) == false)
      {
        Array.Copy(_writeableBitmap.Pixels, _bufferBitmap.Pixels, _writeableBitmap.Pixels.Length);
        _bufferBitmap.Invalidate();

        BufferChanged?.Invoke(this, EventArgs.Empty);
      }
#else //_writeableBitmap.Clear();
//_writeableBitmap.Render(Visual);

      var drawingVisual = new DrawingVisual();
      var drawingContext = drawingVisual.RenderOpen();
      var visualBrush = new VisualBrush(Visual);
      drawingContext.DrawRectangle(visualBrush, null, Visual.GetClientBox());
      drawingContext.Close();

      _writeableBitmap.Clear();
      _writeableBitmap.Render(drawingVisual);

      var stride = _writeableBitmap.PixelWidth * 4;
      _writeableBitmap.CopyPixels(_bufferPixels, stride, 0);

      if (_bufferPixels.SequenceEqual(_pixels) == false)
      {
        Array.Copy(_bufferPixels, 0, _pixels, 0, _bufferPixels.Length);
        _bufferBitmap.WritePixels(new Int32Rect(0, 0, _bufferBitmap.PixelWidth, _bufferBitmap.PixelHeight), _pixels, stride, 0);
      }
#endif
    }

    private void UpdateBitmap()
    {
      var visual = Visual;

      if (visual == null)
      {
        ImageSource = null;
        return;
      }

      if (_writeableBitmap != null && visual.ActualWidth.IsCloseTo(_writeableBitmap.PixelWidth) && visual.ActualHeight.IsCloseTo(_writeableBitmap.PixelHeight) || visual.ActualWidth.Equals(0.0) || visual.ActualHeight.Equals(0.0))
        return;

#if SILVERLIGHT
      _writeableBitmap = new WriteableBitmap((int) visual.ActualWidth, (int) visual.ActualHeight);
      _bufferBitmap = new WriteableBitmap((int) visual.ActualWidth, (int) visual.ActualHeight);
#else
      _writeableBitmap = new RenderTargetBitmap((int) visual.ActualWidth, (int) visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
      _bufferBitmap = new WriteableBitmap((int) visual.ActualWidth, (int) visual.ActualHeight, 96, 96, PixelFormats.Pbgra32, null);

      _pixels = new int[_writeableBitmap.PixelWidth * _writeableBitmap.PixelWidth * 4];
      _bufferPixels = new int[_writeableBitmap.PixelWidth * _writeableBitmap.PixelWidth * 4];
#endif

      ImageSource = _bufferBitmap;
    }

    #endregion
  }
}
