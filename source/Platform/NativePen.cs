// <copyright file="NativePen.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Platform
{
  internal struct NativePen : IDisposable
  {
    #region Fields

    private IntPtr _hpen;

    #endregion

    #region  Methods

    public NativePen(PenStyle fnPenStyle, int nWidth, COLORREF crColor) : this()
    {
      _hpen = NativeMethods.CreatePen(fnPenStyle, nWidth, crColor.Value);
    }

    public IntPtr HPen => _hpen;

    public void Release()
    {
      if (_hpen == IntPtr.Zero) return;

      NativeMethods.DeleteObject(_hpen);
      _hpen = IntPtr.Zero;
    }

    public static explicit operator IntPtr(NativePen brush)
    {
      return brush._hpen;
    }

    #endregion

    public void Dispose()
    {
      Release();
    }
  }
}