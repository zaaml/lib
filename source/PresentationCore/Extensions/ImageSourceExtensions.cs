// <copyright file="ImageSourceExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@xmetropol.com">
//   Copyright (c) xmetropol. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#if SILVERLIGHT
using System.Collections.Generic;
using System.Windows.Media.Imaging;
#endif

namespace Zaaml.PresentationCore.Extensions
{
  internal static class ImageSourceExtensions
  {
    #region  Methods

    internal static void Preload(this ImageSource newIcon)
    {
#if SILVERLIGHT
      var imageSource = newIcon as BitmapImage;
      if (imageSource != null && imageSource.PixelHeight == 0 && imageSource.PixelWidth == 0)
      {
        if (imageSource.UriSource != null && PreloadCache.Add(imageSource.UriSource.OriginalString) == false)
          return;

        ImagePreloadHost.PreloadImage(imageSource);
      }
#endif
    }

    #endregion

    #region  Nested Types

    private class ImagePreloadHostPanel : Panel
    {
      #region Fields

      private readonly Image _image = new Image();
      private readonly Popup _popup = new Popup();

      #endregion

      #region Ctors

      public ImagePreloadHostPanel()
      {
        Children.Add(_image);
        _popup.Child = this;
      }

      #endregion

      #region  Methods

      protected override Size MeasureOverride(Size availableSize)
      {
        _image.Measure(XamlConstants.InfiniteSize);
        return XamlConstants.ZeroSize;
      }

      protected override Size ArrangeOverride(Size finalSize)
      {
        _image.Arrange(finalSize.Rect());
        return finalSize;
      }

      public void PreloadImage(ImageSource image)
      {
        _image.Source = image;
        _popup.IsOpen = true;
        InvalidateMeasure();
      }

      #endregion
    }

    #endregion

#if SILVERLIGHT

    private static readonly ImagePreloadHostPanel ImagePreloadHost = new ImagePreloadHostPanel();

    private static readonly HashSet<string> PreloadCache = new HashSet<string>();
#endif
  }


}