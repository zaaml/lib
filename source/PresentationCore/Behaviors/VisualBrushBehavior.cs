// <copyright file="VisualBrushBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
  public class VisualBrushBehavior : BehaviorBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SourceProperty = DPM.Register<FrameworkElement, VisualBrushBehavior>
      ("Source", b => b.OnSourceChanged);

    public static readonly DependencyProperty ModeProperty = DPM.Register<VisualBrushMode, VisualBrushBehavior>
      ("Mode", VisualBrushMode.RefreshOnRender, b => b.OnModeChanged);

    private static readonly DependencyPropertyKey ImageSourcePropertyKey = DPM.RegisterReadOnly<ImageSource, VisualBrushBehavior>
      ("ImageSource");

    private static readonly DependencyPropertyKey BrushPropertyKey = DPM.RegisterReadOnly<Brush, VisualBrushBehavior>
      ("Brush");

    public static readonly DependencyProperty BrushProperty = BrushPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ImageSourceProperty = ImageSourcePropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private CompositionRenderingObserver _renderingObserver;

    #endregion

    #region Properties

    public Brush Brush
    {
      get => (Brush) GetValue(BrushProperty);
      private set => this.SetReadOnlyValue(BrushPropertyKey, value);
    }

    public ImageSource ImageSource
    {
      get => (ImageSource) GetValue(ImageSourceProperty);
      private set => this.SetReadOnlyValue(ImageSourcePropertyKey, value);
    }

    public VisualBrushMode Mode
    {
      get => (VisualBrushMode) GetValue(ModeProperty);
      set => SetValue(ModeProperty, value);
    }

    public FrameworkElement Source
    {
      get => (FrameworkElement) GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    #endregion

    #region  Methods

    private void AttachRendering()
    {
	    _renderingObserver = new CompositionRenderingObserver(OnRendering);
    }

    private void DetachRendering()
    {
	    _renderingObserver = _renderingObserver.DisposeExchange();
    }

    protected override void OnAttached()
    {
      base.OnAttached();

      if (Mode == VisualBrushMode.RefreshOnRender)
        AttachRendering();

      if (Source != null)
        Source.SizeChanged += OnElementSizeChanged;

      Update();
    }

    protected override void OnDetaching()
    {
      Reset();

      DetachRendering();
      if (Source != null)
        Source.SizeChanged -= OnElementSizeChanged;

      base.OnDetaching();
    }

    private void OnElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
      Update();
    }

    private void OnModeChanged(VisualBrushMode oldMode, VisualBrushMode newMode)
    {
      if (FrameworkElement == null)
        return;

      if (oldMode == VisualBrushMode.RefreshOnRender)
        DetachRendering();

      if (newMode == VisualBrushMode.RefreshOnRender)
        AttachRendering();

      if (Mode == VisualBrushMode.Disabled)
        RefreshBitmap();
    }

    private void OnRendering()
    {
      RefreshBitmap();
    }

    private void OnSourceChanged(FrameworkElement oldElement, FrameworkElement newElement)
    {
#if SILVERLIGHT
      if (FrameworkElement == null)
        return;

      if (oldElement != null)
        oldElement.SizeChanged -= OnElementSizeChanged;
      if (newElement != null)
        newElement.SizeChanged += OnElementSizeChanged;

      Update();
#else
      
      Brush = Source != null ? new VisualBrush(Source) : null;
      
      //ImageSource = new DrawingImage {Drawing = new ImageDrawing() {} };
#endif
    }

    private void RefreshBitmap()
    {
#if SILVERLIGHT
			if (Source == null)
				return;

			var bitmap = (WriteableBitmap) ImageSource;

			if (bitmap == null)
				return;

			if (Mode == VisualBrushMode.Disabled)
			{
				Array.Clear(bitmap.Pixels, 0, bitmap.Pixels.Length);
				bitmap.Invalidate();
			}
			else
			{
				Array.Clear(bitmap.Pixels, 0, bitmap.Pixels.Length);
				bitmap.Render(Source, Source.RenderTransform);
				bitmap.Invalidate();
			}
#endif
    }

    private void Reset()
    {
      ImageSource = null;
      Brush = null;
    }

    private void Update()
    {
#if SILVERLIGHT
			if (Source == null)
				Reset();
			else
			{
				var writeableBitmap = new WriteableBitmap((int) Source.ActualWidth, (int) Source.ActualHeight);
				ImageSource = writeableBitmap;

				if (Brush == null)
					Brush = new ImageBrush {ImageSource = ImageSource};
				else
					((ImageBrush) Brush).ImageSource = ImageSource;

				RefreshBitmap();
			}
#endif
    }

    #endregion
  }
}