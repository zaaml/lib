// <copyright file="WriteableBitmapUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Zaaml.PresentationCore.Utils
{
  internal static class WriteableBitmapUtils
  {
    #region  Methods

    public static WriteableBitmap Create(Size size)
    {
      return Create((int) size.Width, (int) size.Height);
    }

    public static WriteableBitmap Create(int width, int height)
    {
#if SILVERLIGHT
			return new WriteableBitmap(width, height);
#else
      return new WriteableBitmap(width, height, DpiUtils.DpiX, DpiUtils.DpiY, PixelFormats.Pbgra32, null);
#endif
    }

    #endregion
  }
}