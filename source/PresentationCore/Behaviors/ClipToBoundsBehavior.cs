// <copyright file="ClipToBoundsBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Behaviors
{
  public class ClipToBoundsBehavior : BehaviorBase
  {
    #region Fields

    private readonly RectangleGeometry _clipGeometry = new RectangleGeometry();

    #endregion

    #region  Methods

    protected override void OnAttached()
    {
      base.OnAttached();

      UpdateClipGeometry();
      FrameworkElement.Clip = _clipGeometry;

      FrameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
    }

    protected override void OnDetaching()
    {
      if (ReferenceEquals(FrameworkElement.Clip, _clipGeometry))
        FrameworkElement.ClearValue(UIElement.ClipProperty);

      FrameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;

      base.OnDetaching();
    }

    private void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateClipGeometry();
    }

    private void UpdateClipGeometry()
    {
      _clipGeometry.Rect = FrameworkElement.GetClientBox();
    }

    #endregion
  }
}