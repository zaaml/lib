// <copyright file="GarbageCleanupCounter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  internal static class GarbageCleanupCounter
  {
    #region Static Fields and Constants

    private static int _cleanupCount;
    private static WeakReference _gc = new WeakReference(new object());

    #endregion

    #region Properties

    public static int CleanupCount
    {
      get
      {
        if (_gc.IsAlive)
          return _cleanupCount;

        _gc = new WeakReference(new object());
        _cleanupCount++;

        return _cleanupCount;
      }
    }

    #endregion
  }
}