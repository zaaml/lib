// <copyright file="sRgbColor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.ColorModel
{
  internal struct sRgbColor
  {
    public byte A;
    public byte B;
    public byte G;
    public byte R;

    public sRgbColor(byte a, byte r, byte g, byte b)
    {
      A = a;
      R = r;
      G = g;
      B = b;
    }
  }
}