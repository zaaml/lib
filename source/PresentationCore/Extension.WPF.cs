// <copyright file="Extension.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore
{
  public static partial class Extension
  {
    #region  Methods

    internal static ImageSource CreateBitmap(FrameworkElement element, Size size)
    {
      var writeableBitmap = new RenderTargetBitmap((int) size.Width, (int) size.Height, DpiUtils.DpiX, DpiUtils.DpiY, PixelFormats.Pbgra32);

      element.Measure(size);
      element.Arrange(size.Rect());

      writeableBitmap.Render(element);

      return writeableBitmap;
    }

    static partial void PlatformCtor()
    {
    }

    #endregion
  }
}