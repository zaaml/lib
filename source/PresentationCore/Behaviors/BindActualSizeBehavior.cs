// <copyright file="BindActualSizeBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
  public class BindActualSizeBehavior : BehaviorBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SourceProperty = DPM.Register<FrameworkElement, BindActualSizeBehavior>
      ("Source", b => b.OnSourceChanged);

    public static readonly DependencyProperty FallbackWidthProperty = DPM.Register<double, BindActualSizeBehavior>
      ("FallbackWidth");

    public static readonly DependencyProperty FallbackHeightProperty = DPM.Register<double, BindActualSizeBehavior>
      ("FallbackHeight");

    #endregion

    #region Properties

    public double FallbackHeight
    {
      get => (double) GetValue(FallbackHeightProperty);
      set => SetValue(FallbackHeightProperty, value);
    }

    public double FallbackWidth
    {
      get => (double) GetValue(FallbackWidthProperty);
      set => SetValue(FallbackWidthProperty, value);
    }

    public FrameworkElement Source
    {
      get => (FrameworkElement) GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    #endregion

    #region  Methods

    protected override void OnAttached()
    {
      base.OnAttached();
      SetSize();
    }

    private void OnSourceChanged(FrameworkElement oldElement, FrameworkElement newElement)
    {
      if (oldElement != null)
        oldElement.SizeChanged -= OnSourceSizeChanged;

      if (newElement != null)
        newElement.SizeChanged += OnSourceSizeChanged;

      SetSize();
    }

    private void OnSourceSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    {
      SetSize();
    }

    private void SetSize()
    {
      FrameworkElement?.SetSize(Source.Return(s => s.GetActualSize(), () => new Size(FallbackWidth, FallbackHeight)));
    }

    #endregion
  }
}